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
import imp
import re
import string
import csv
import codecs
from imp import reload
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

EXPORT_CLIENT = True
EXPORT_SERVER = True

server_export_excel_list = []
client_export_excel_list = []


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
		if sdict: luaT[sidx] = sdict

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

def write_table(luaT, luaN ,outfile = '-', withfunc = True):
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
		# outfp.write("[%d] = {\n"%(sidx + 1))
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
						
						if name_head.find("int32[") > 0:
							col=str(col)

						if type(col) is int: s = "%d"%(col)
						elif type(col) is float: s = FLOAT_FORMAT%(col)
						else : 
							szCol = col.strip();

							if szCol[0] == '{' and szCol[(len(szCol)-1)] == '}':
								s = "%s" % (col)
							else:
								s = "\"%s\""%(format_output(col))
								# s = "[[%s]]"%(format_output(col))		--2015/06/03支持换行
						# outfp.write("\t\t[%d] = %s,\n"%(colidx + 1, s))
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
	if not os.path.exists(dir_dst) and EXPORT_CLIENT:
		os.makedirs(dir_dst)

	#tag = True
	for name in client_export_excel_list:
		name_temp = dir_src + "\\" + name
		'''
		if os.path.isdir(name_temp): 
			if tag is True:
				dir_temp = dir_dst + name
				transferClient(name_temp, dir_temp)
			else:
				continue
		'''
		if os.path.isfile(name_temp) and not name.startswith("_") and name.endswith("xlsx") and not '$' in name :
			print("Start : " + name_temp)
			t, n = gen_table(name_temp)
			if EXPORT_CLIENT :
				if name.startswith('s_') or name.startswith('S_'):
					continue
				pathInfo =  os.path.basename(name_temp)
				outfile = dir_dst + pathInfo.split('.')[0] + ".lua"
				write_table(t, n, outfile, withfunc = True)
				print("Client : SUCCESS ")

def export_server_csv(dir_src, dir_dst):
	if dir_dst[len(dir_dst) - 1] != '\\':
		dir_dst = dir_dst + "\\"
	# 创建dst_src的目录树
	if not os.path.exists(dir_dst) and EXPORT_CLIENT:
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
			
		if os.path.isfile(name_temp) and not name.startswith("_") and name.endswith("xlsx") and not '$' in name :
			print("Start : " + name_temp)
			t, n = gen_table(name_temp)
			if EXPORT_SERVER:
				if name.startswith('c_') or name.startswith('C_'):
					continue
				pathInfo =  os.path.basename(name_temp)
				outfile = dir_dst + pathInfo.split('.')[0] + ".csv"
				transferServer(name_temp,outfile)
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
		is_export_client = sheet.cell_value(ridx, 1)
		is_export_server = sheet.cell_value(ridx, 2)
        
		if int(is_export_client) :
			client_export_excel_list.append(export_filename+".xlsx")
		if int(is_export_server) :
			server_export_excel_list.append(export_filename+".xlsx")
		
def main():
	#读取ini配置文件  依次获得excel目录  服务器导出目录 客户端导出目录
	config_parse = configparser.ConfigParser()
	config_ini = r"ExcelTools.ini"
	config_parse.read(config_ini)
	dir_src=config_parse.get("config","excel_dir")
	dir_dst_client = config_parse.get("config","client_export_dir")
	dir_dst_server = config_parse.get("config","server_export_dir")
	export_config = config_parse.get("config","excel_export_config")
	get_export_excel_list(dir_src+"//"+export_config)
	export_client_lua(dir_src,dir_dst_client)
	export_server_csv(dir_src,dir_dst_server)

if __name__=="__main__":
	main()
