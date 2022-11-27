# -*- coding: UTF-8 -*-
import sys, os, re
import pystache
config = {
    "user" : 100,
    "login" : 500,
    "battle" : 1000
}


c = {
    'indexToName': []
}



def main():
    for root, dirs, files in os.walk('.'):
        for name in files:
            basename = os.path.splitext(name)[0]
            extname = os.path.splitext(name)[1]
            if extname != ".proto":
                continue
            if basename not in config:
                continue

            print ("import proto (%s) is (%s) is (%s)" %(name, basename, extname))
            currentIndex = config[basename] + 1
            f = open(name,encoding='gb18030',errors='ignore')
            content = f.read()
            pattern = re.compile(r'package (\w+);')
            match = pattern.search(content)
            if not match:
                print ("not match package name (%s)" % (name))
                return

            if match.group() != basename:
                print ("not match package name %s != %s" % (name, match.group()))
                
            pattern = re.compile(r'message (\w+)')
            match = pattern.search(content)
            if match:
                for message in re.findall(pattern, content):
                    totalName = "%s.%s" % (basename, message)
                    data1 = {}
                    data1["index"] = currentIndex
                    data1["name"] = totalName
                    currentIndex = currentIndex + 1
                    c["indexToName"].append(data1)


    r = pystache.Renderer()
    baseModel = open('protocols.mustache', 'r')
    baseContent = pystache.render(baseModel.read(), c)

    f1 = open('protocols.lua','w')
    f1.write(baseContent)
    f1.close()

if __name__=="__main__":
    main()
