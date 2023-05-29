#! /usr/bin/env python
# -*- coding: utf8	-*-

# convert excel xls file to lua script with table data
# date/time values formatted as string, int values formatted as int
# depend on xlrd module
# author: fanlix 2008.1.25
# Modify: 
# page@2015/04/09 tolua
# page@2015/04/12 support directory recursively
# page@2015/04/12 gbk-->utf-8

import xlrd
import os.path

#2015/04/09 解决编码问题
import sys
import importlib
import re
import string
import csv
import json
import codecs
from importlib import reload
import configparser
reload(sys)

#==========================================================================
# 默认配置
DIR_SRC = "src"
DIR_DST = "dst"

#excel数据起始行
ROW_START = 3
#==========================================================================
FLOAT_FORMAT = "%.8f"

EXPORT_CLIENT_LUA = True
EXPORT_CLIENT_CSHARP = True
EXPORT_CLIENT_JSON = True
EXPORT_SERVER = True

server_export_excel_list = []
client_export_lua_excel_list = []
client_export_csharp_excel_list = []


def gen_table(filename):
	if not os.path.isfile(filename):
		raise Exception( "%s is	not	a valid	filename" % filename)
	xlrd.Book.encoding = "utf-8"
	book = xlrd.open_workbook(filename)
	luaT = {}
	luaN = {}

	sidx = 0
	for sheet in book.sheets():
		if sidx >= 1: 
			break;
		sdict = {}
		ridx = 0
		for ridx in range(sheet.nrows):
			rdict = {}
			for cidx in range(sheet.ncols):
				value = sheet.cell_value(ridx, cidx)
				vtype = sheet.cell_type(ridx, cidx)
				v = format_value(value, vtype, book)
				#print sidx, ridx, cidx, value, vtype, v
				if v is not None and value != "":
					rdict[cidx] = v
			if rdict: sdict[ridx] = rdict
		if sdict: 
			luaT[sidx] = sdict

		# handle merged-cell
		for crange in sheet.merged_cells:
			rlo, rhi, clo, chi = crange
			try:
				v = sdict[rlo][clo]
			except KeyError:
				# empty cell
				continue
			if v is None or v == "": continue
			for ridx in range(rlo, rhi):
				if ridx not in sdict:
					sdict[ridx] = {}
				for cidx in range(clo, chi):
					sdict[ridx][cidx] = v
		name = sheet.name
		luaN[name] = sidx
		luaT[sidx] = sdict
		sidx += 1
	return luaT, luaN

def tabletodict(luaT):
	allList = {}
	for sidx, sheet in luaT.items():
		#第一行为参考
		head = sheet.get(1)
		if head is None:
			break;
		allIdList = []
		id_index = 0
		for rowidx, row in sheet.items():
			if rowidx >= (ROW_START-1) and row.get(0) != None:
				item = {}
				titleList = [] 
				isUnion = False
				unionID = ""
				for colidx, col in row.items():
					try:
						name_head = head.get(colidx)
						if name_head.startswith("_"):
							continue
						if name_head.find("(int32)") > 0:
							col=int(col)
						if name_head.find("(float)") > 0:
							col=float(col)
						if name_head.find("(bool)") > 0:
							col=bool(col)
						if name_head.find("int32[") > 0:							
							cols = col.split(',')
							col = list(map(int,cols))
						if name_head.find("string[") > 0:							
							cols = col.split(',')
							col=cols
						if name_head.find("float[") > 0:							
							cols = col.split(',')
							col = list(map(float,cols))
						
						s = col		
						name = head.get(colidx)
						if not name.startswith("_"):
							endBlone = name.split("_",1)[0]
							if trim(endBlone).lower() == "p" or trim(endBlone).lower() == "c":
								content = name.split("_",1)[1]

								if "_union" in content.lower():
									isUnion = True
									unionID = unionID + "_" + s
								typeInfo = content.split("(",1)[1].replace(")","")
								typeName = content.split("(",1)[0].replace("_union","")
								
								#处理表头的大小写
								if trim(typeName.upper()) == "ID" :
									# typeName = "ID"
									id_index = colidx
								
								if typeName in titleList :
									raise Exception( " 表头信息重复   " + typeName)

								titleList.append(typeName)
								# itemInfoStr.append("\t\t%s = %s,\n"%(typeName, s))
								item[typeName] = s
					except Exception as e:
						raise Exception("Write Table error (%s,%s,%s) : %s"%(sidx+1,rowidx+1,colidx+1,str(e)))
				if isUnion :
					if unionID.lstrip('_').strip() in allIdList:
						raise Exception( " 联合主键ID重复   " + unionID.lstrip('_').strip())
					allIdList.append(unionID.lstrip('_').strip())
					allList[str(unionID.lstrip('_').strip())] = item
				else:
					if row.get(id_index) in allIdList:
						raise Exception( " 联合主键ID重复   " + row.get(0) )
					allIdList.append(row.get(id_index))
					if is_number(row.get(id_index)):
						allList[int(row.get(id_index))] = item
					else:
						allList[str(row.get(id_index))] = item
	return allList
	

def format_value(value, vtype, book):
	''' format excel cell value, int?date?
	'''
	if vtype == 2:
		if value == int(value):
			value = int(value)
		elif type(value) == float :
			pass
	elif vtype == 3:
		datetuple =	xlrd.xldate_as_tuple(value,	book.datemode)
		# time only	no date	component
		if datetuple[0]	== 0 and datetuple[1] == 0 and datetuple[2] == 0:
			value =	"%02d:%02d:%02d" % datetuple[3:]
		# date only, no	time
		elif datetuple[3] == 0 and datetuple[4]	== 0 and datetuple[5] == 0:
			value =	"%04d/%02d/%02d" % datetuple[:3]
		else: #	full date
			value =	"%04d/%02d/%02d	%02d:%02d:%02d"	% datetuple
	return value

def format_output(v):
	s = ("%s"%(v))
	if s[-1] == "]":
		s = "%s "%(s)
	return s

def write_lua(luaT, luaN ,outfile = '-', withfunc = True):
	''' lua table key index starts from 1
	'''
	if outfile and outfile != '-':
		outfp = open(outfile, 'w',encoding= "utf-8")
	else:
		import StringIO
		outfp = StringIO.StringIO()
	szName = outfile.split('.')[0]
	szName = szName.replace('/', '\\')
	szName = szName.split('\\')[-1]

	outfp.write("return {\n" )

	for sidx, sheet in luaT.items():
		#第一行为参考
		head = sheet.get(1)
		if head is None:
			break;
		max_row = len(head)

		allIdList = []
		id_index = 0

		for rowidx, row in sheet.items():
			#Notify: from row 3 start; index start from 0
			if rowidx >= (ROW_START-1) and row.get(0) != None:
				itemInfoStr = []
				titleList = [] 
				isUnion = False
				unionID = ""
				for colidx, col in row.items():
					try:
						name_head = head.get(colidx)
						if name_head.startswith("_"):
							continue
						if name_head.find("(int32)") > 0:
							col=int(col)
						if name_head.find("(bool)") > 0:
							col=bool(col)
						if name_head.find("int32[") > 0:
							col=str(col)
							col = "{" + col + "}"
						if name_head.find("string[") > 0:
							col=str(col)
							col = "{\"" + col.replace(",", "\",\"") + "\"}"
						if name_head.find("float[") > 0:
							col=str(col)
							col = "{" + col + "}"
						if type(col) is int: s = "%d"%(col)
						elif type(col) is float: s = FLOAT_FORMAT%(col)
						elif type(col) is bool: 
							s=str(col).lower()
						else : 
							szCol = col.strip();
							if szCol[0] == '{' and szCol[(len(szCol)-1)] == '}':
								s = "%s" % (col)
							else:
								s = "\"%s\""%(format_output(col))
						name = head.get(colidx)

						if not name.startswith("_"):
							endBlone = name.split("_",1)[0]
							if trim(endBlone).lower() == "p" or trim(endBlone).lower() == "c":
								content = name.split("_",1)[1]

								if "_union" in content.lower():
									isUnion = True
									unionID = unionID + "_" + s
								typeInfo = content.split("(",1)[1].replace(")","")
								typeName = content.split("(",1)[0].replace("_union","")

								
								#处理表头的大小写
								if trim(typeName.upper()) == "ID" :
									typeName = "ID"
									id_index = colidx
								
								if typeName in titleList :
									raise Exception( " 表头信息重复   " + typeName)

								titleList.append(typeName)
								itemInfoStr.append("\t\t%s = %s,\n"%(typeName, s))
					except Exception as e:
						raise Exception("Write Table error (%s,%s,%s) : %s"%(sidx+1,rowidx+1,colidx+1,str(e)))
					
				if isUnion :
					if unionID.lstrip('_').strip() in allIdList:
						raise Exception( " 联合主键ID重复   " + unionID.lstrip('_').strip())
					allIdList.append(unionID.lstrip('_').strip())
					itemInfoStr.insert(0,"\t[\"%s\"] = {\n"%(unionID.lstrip('_').strip()))
				else:
					if row.get(id_index) in allIdList:
						raise Exception( " 联合主键ID重复   " + row.get(0) )
					allIdList.append(row.get(id_index))
					if is_number(row.get(id_index)):
						itemInfoStr.insert(0,"\t[%d] = {\n"%(int(row.get(id_index))))
					else:
						itemInfoStr.insert(0,"\t[\"%s\"] = {\n"%(row.get(id_index)))
				
				outfp.write(''.join(itemInfoStr))
				outfp.write("\t},\n")
	outfp.write("};\n\n")

	if not outfile or outfile == '-':
		outfp.seek(0)
		print(outfp.read())
	outfp.close()

def write_json(luaT, luaN ,outfile = '-', withfunc = True):
	if outfile and outfile != '-':
		outfp = open(outfile, 'w',encoding= "utf-8")
	else:
		import StringIO
		outfp = StringIO.StringIO()

	allList = tabletodict(luaT)
	data = json.dumps(allList, indent=1, ensure_ascii=False)
	outfp.write(data)
	if not outfile or outfile == '-':
		outfp.seek(0)
		print(outfp.read())
	outfp.close()


def write_csharp(luaT, luaN ,outfile = '-', withfunc = True):
	if outfile and outfile != '-':
		outfp = open(outfile, 'w',encoding= "utf-8")
	else:
		import StringIO
		outfp = StringIO.StringIO()
	szName = outfile.split('.')[2]
	szName = szName.split('\\')[-1]
	tmpconfig = open("TmpConfig.txt",'r')
	txttmpconfig = tmpconfig.read()
	txttmpconfig = txttmpconfig.replace("(ConfigName)",szName)
	from io import StringIO
	fields = StringIO()
	allList = tabletodict(luaT)
	for key, value in allList.items():
		for k, v in value.items():
			if type(v) is int:
				fields.write("\tpublic int "+ k +";\n")
			elif type(v) is float: 
				fields.write("\tpublic float "+ k +";\n")
			elif type(v) is bool:
				 fields.write("\tpublic bool "+ k +";\n")
			elif type(v) is str:
				 fields.write("\tpublic string "+ k +";\n")
			else:
				if type(v[0]) is int:
					fields.write("\tpublic List<int> "+ k +";\n")
				elif type(v[0]) is float:
					fields.write("\tpublic List<float> "+ k +";\n")
				else:
					fields.write("\tpublic List<string> "+ k +";\n")
		break
	txttmpconfig = txttmpconfig.replace("(Fields)", fields.getvalue())
	fields.close()
	outfp.write(txttmpconfig)
	if not outfile or outfile == '-':
		outfp.seek(0)
		print(outfp.read())
	outfp.close()

def trim(s):
	if s.startswith(' ') or s.endswith(' '):
		return re.sub(r"^(\s+)|(\s+)$", "", s)
	return s

def is_number(s):
    try:
        float(s)
        return True
    except ValueError:
        pass
 
    try:
        import unicodedata
        unicodedata.numeric(s)
        return True
    except (TypeError, ValueError):
        pass
    return False

def transferServer(dir_src, dir_dst):
	workbook = xlrd.open_workbook(dir_src)
	table = workbook.sheet_by_index(0)
	with codecs.open(dir_dst, 'w', encoding='utf-8') as f:
		write = csv.writer(f)
		for row_num in range(table.nrows):
			row_value = table.row_values(row_num)
			write.writerow(row_value)

def export_client_lua(dir_src,dir_dst):
	if dir_dst[len(dir_dst) - 1] != '\\':
		dir_dst = dir_dst + "\\"
	# 创建dst_src的目录树
	if not os.path.exists(dir_dst) and EXPORT_CLIENT_LUA:
		os.makedirs(dir_dst)

	#tag = True
	for name in client_export_lua_excel_list:
		name_temp = dir_src + "\\" + name
		if os.path.isfile(name_temp) and not name.startswith("_") and not '$' in name :
			print("Start Lua: " + name_temp)
			t, n = gen_table(name_temp)
			if EXPORT_CLIENT_LUA :
				if name.startswith('s_') or name.startswith('S_'):
					continue
				pathInfo =  os.path.basename(name_temp)
				outfile = dir_dst + pathInfo.split('.')[0] + ".lua.bytes"
				write_lua(t, n, outfile, withfunc = True)
				print("Client Lua: SUCCESS ")

def export_client_csharp(dir_src, dir_dst_csharp):
	if dir_dst_csharp[len(dir_dst_csharp) - 1] != '\\':
		dir_dst_csharp = dir_dst_csharp + "\\"
	# 创建dst_src的目录树
	if not os.path.exists(dir_dst_csharp) and EXPORT_CLIENT_CSHARP:
		os.makedirs(dir_dst_csharp)
	for name in client_export_csharp_excel_list:
		name_temp = dir_src + "\\" + name
		if os.path.isfile(name_temp) and not name.startswith("_") and not '$' in name :
			print("Start Csharp: " + name_temp)
			t, n = gen_table(name_temp)
			if EXPORT_CLIENT_LUA :
				if name.startswith('s_') or name.startswith('S_'):
					continue
				pathInfo =  os.path.basename(name_temp)
				write_csharp(t, n, dir_dst_csharp + pathInfo.split('.')[0] + "Config.cs", withfunc = True)
				print("Client Csharp: SUCCESS ")

# 生成manager
def export_client_csharp_manager(dir_src, dir_dst_manager):
	outfile = dir_dst_manager + "\ConfigManager.cs"
	if outfile and outfile != '-':
		outfp = open(outfile, 'w',encoding= "utf-8")
	from io import StringIO
	configvar = StringIO()
	configinit = StringIO()	
	tmpmanager = open("TmpManager.txt",'r')
	txttmpmanager = tmpmanager.read()
	for filename in client_export_csharp_excel_list:
		name = filename.split('.')[0]
		configname = name + "Config"
		confignamelower = configname[0].lower() + configname[1:]
		configvar.write("public " + configname + " " + confignamelower + " = new " + configname + "();\n")
		configinit.write(confignamelower + ".Init(LoadConfig<List<ChineseTextConfigItem>>(\"" + name + "\"));\n")
	txttmpmanager = txttmpmanager.replace("[CONFIG_INIT]", configinit.getvalue())
	txttmpmanager = txttmpmanager.replace("[CONFIG_VAR]", configvar.getvalue())
	tmpmanager.close()
	outfp.write(txttmpmanager)
	if not outfile or outfile == '-':
		outfp.seek(0)
		print(outfp.read())
	outfp.close()	

def export_client_json(dir_src, dir_dst_json):
	if dir_dst_json[len(dir_dst_json) - 1] != '\\':
		dir_dst_json = dir_dst_json + "\\"
	# 创建dst_src的目录树
	if not os.path.exists(dir_dst_json) and EXPORT_CLIENT_JSON:
		os.makedirs(dir_dst_json)
	for name in client_export_csharp_excel_list:
		name_temp = dir_src + "\\" + name
		if os.path.isfile(name_temp) and not name.startswith("_") and not '$' in name :
			print("Start Json: " + name_temp)
			t, n = gen_table(name_temp)
			if EXPORT_CLIENT_LUA :
				if name.startswith('s_') or name.startswith('S_'):
					continue
				pathInfo =  os.path.basename(name_temp)
				write_json(t, n, dir_dst_json + pathInfo.split('.')[0] + ".json", withfunc = True)
				print("Client Json: SUCCESS ")

def export_server_json(dir_src, dir_dst):
	if dir_dst[len(dir_dst) - 1] != '\\':
		dir_dst = dir_dst + "\\"
	# 创建dst_src的目录树
	if not os.path.exists(dir_dst) and EXPORT_SERVER:
		os.makedirs(dir_dst)

	#tag = True
	for name in server_export_excel_list:
		name_temp = dir_src + "\\" + name
		'''
		if os.path.isdir(name_temp): 
			if tag is True:
				dir_temp = dir_dst + name
				transferClient(name_temp, dir_temp)
			else:
				continue
		'''
			
		if os.path.isfile(name_temp) and not name.startswith("_") and not '$' in name :
			print("Start : " + name_temp)
			t, n = gen_table(name_temp)
			if EXPORT_SERVER:
				if name.startswith('c_') or name.startswith('C_'):
					continue
				pathInfo =  os.path.basename(name_temp)
				# outfile = dir_dst + pathInfo.split('.')[0] + ".csv"
				# transferServer(name_temp,outfile)
				write_json(t, n, dir_dst + pathInfo.split('.')[0] + ".json", withfunc = True)
				print("SERVER : SUCCESS ")
			


def get_export_excel_list(export_file):
	if not os.path.isfile(export_file):
		raise Exception( "%s is	not	a valid	filename" % export_file)
	xlrd.Book.encoding = "utf-8"
	book = xlrd.open_workbook(export_file)
	sheet = book.sheet_by_index(0)

	#然后读取sheet
	for ridx in range(1,sheet.nrows):
		export_filename  = sheet.cell_value(ridx, 0)
		export_client_lua = sheet.cell_value(ridx, 1)
		export_client_csharp = sheet.cell_value(ridx, 2)
		is_export_server = sheet.cell_value(ridx, 3)
        
		if int(export_client_lua) :
			client_export_lua_excel_list.append(export_filename+".xls")
		if int(export_client_csharp) :
			client_export_csharp_excel_list.append(export_filename+".xls")
		if int(is_export_server) :
			server_export_excel_list.append(export_filename+".xls")
		
def main():
	#读取ini配置文件  依次获得excel目录  服务器导出目录 客户端导出目录
	config_parse = configparser.ConfigParser()
	config_ini = r"ExcelTools.ini"
	config_parse.read(config_ini)
	dir_src=config_parse.get("config","excel_dir")
	dir_dst_client_lua = config_parse.get("config","client_export_lua_dir")
	dir_dst_client_csharp = config_parse.get("config","client_export_csharp_dir")
	dir_dst_client_csharp_manager = config_parse.get("config","client_export_csharp_manager_dir")
	dir_dst_client_json = config_parse.get("config","client_export_json_dir")
	dir_dst_server = config_parse.get("config","server_export_dir")
	export_config = config_parse.get("config","excel_export_config")
	get_export_excel_list(dir_src+"//"+export_config)
	export_client_lua(dir_src,dir_dst_client_lua)
	export_client_csharp(dir_src, dir_dst_client_csharp)
	export_client_csharp_manager(dir_src, dir_dst_client_csharp_manager)
	export_client_json(dir_src, dir_dst_client_json)
	export_server_json(dir_src,dir_dst_server)

if __name__=="__main__":
	main()
