#!/usr/bin/python
# -*- coding: UTF-8 -*-
# 目前暂时不支持枚举值的导出TODO:

import sys, os, re
config = {
    "user" : 100,
    "login" : 500,
    "battle" : 1000
}

#--------------------------------------------
# 替换字符串方法
#--------------------------------------------
#define a function  
def ReplaceStr(s):
    s = s.replace('=', ' ');    #先把所有的'='替换成'空格'
    s = s.replace('=', ' ');    #先把所有的'='替换成'空格'
    s = s.replace('{', ' ');    #先把所有的'{'替换成'空格'
    s = s.replace('\t', ' ');   #先把所有的'\t'替换成'空格'
    s = re.sub(r'\s+', '-', s); #把任意多个'空格'替换成'-'
    return s
    
#--------------------------------------------
# 检测类型赋值
#--------------------------------------------
#define a function  
def GetTypeValue(s):
    if t == "repeated":
        return "{}"
        
    typeValue = ""
    if s == "int32":
        typeValue = "0"
    elif s == "int64":
        typeValue = "0"
    elif s == "string":
        typeValue = "\"\""
    elif s == "string":
        typeValue = "\"\""
    else:
        typeValue = "{}"
    return typeValue

#--------------------------------------------
# 检测类型替换后的字符串
#--------------------------------------------
#define a function  
def CheckType(s,clsStr):
    rtStr = ""
    arrSplit = s.split('-')
    newCls = ""
    if arrSplit[0] == "message":
        newCls = arrSplit[1]
        rtStr += "\n---@class " + newCls + '\n'
        rtStr += "CMD." + newCls + " = {}"
        rtStr += '\n'
    elif (len(arrSplit) > 3) and (arrSplit[1] == "required" or arrSplit[1] == "repeated" or arrSplit[1] == "optional"):
        rtStr += "CMD." + clsStr + "." + arrSplit[3] + " = " + GetTypeValue(arrSplit[1], arrSplit[2])
        rtStr += '\n'
    #elif arrSplit[0] == '}':
        #rtStr += '}'
    return rtStr, newCls

#--------------------------------------------
# 读取文件
#--------------------------------------------
for root, dirs, files in os.walk('.'):
    for name in files:
        basename = os.path.splitext(name)[0]
        extname = os.path.splitext(name)[1]
        if extname != ".proto":
            continue
        if basename not in config:
            continue

        allStr = "local CMD = {}"    #拼接的字符串,用来写文件
        lastClass = "" #记录上一个ClassName
        fileRead = open("name")             # 返回一个文件对象  
        line = fileRead.readline()             # 调用文件的 readline()方法
        while line:
            rptStr = ""
            rptStr = ReplaceStr(line)          #自己写的替换字符串
            rptStr,tmpCls = CheckType(rptStr,lastClass)
            if tmpCls != "":
                lastClass = tmpCls
            
            allStr += rptStr
            line = fileRead.readline()

        allStr = allStr + " return CMD"
        #--------------------------------------------
        # 写文件
        #--------------------------------------------
        targetName = basename + "_proto.lua"
        fileWrite = open(targetName, 'w')
        fileWrite.write(allStr)
        fileWrite.close()
        print ("proto文件导成lua文件成功:%s" % targetName)



