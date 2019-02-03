#!/usr/bin/python
#coding=utf-8

import os,sys
import re

os.chdir(os.path.abspath(sys.path[0]))
reload(sys)
sys.setdefaultencoding('utf-8')

LangCNPath = "../StreamingAssets/Lang/Lang_CN.txt"

MatchPattern = None
StrInfo_List = []

def GetMatchPattern():
	global MatchPattern
	if not MatchPattern:
		MatchPattern = re.compile(ur'\"([\u4e00-\u9fa5:]+)\"')
	pass
	return MatchPattern
pass

def CheckIsStr(file_name, line):
	pattern = GetMatchPattern()
	results = pattern.findall(line.decode('utf-8'))
	if len(results) >= 1:
		# print file_name, results
		return True
	else:
		return False
	pass
pass

def FindAllStrPath():
	for root_path, dirs, files in os.walk("../Scripts/Client/View"):
		for file in files:
			file_path = "%s%s%s" % (root_path, os.sep, file)
			str_list = os.path.splitext(file)
			file_name = str_list[0]
			file_format = str_list[1]

			if file_format == ".cs":
				file_stream = open(file_path, 'r')
				lines = file_stream.readlines()
				for line in lines:
					if CheckIsStr(file_name, line):
						str_info = {}
						str_info["FilePath"] = file_path
						str_info["FileName"] = file_name
						StrInfo_List.append(str_info)
						break
					pass
				pass
			pass
		pass
	pass
pass

def CreateLangStrFile():
	str_file_stream = open(LangCNPath, 'w+')

	# 文字编号
	str_idx = 0
	for str_info in StrInfo_List:
		file_stream = open(str_info["FilePath"], 'r+')
		lines = file_stream.readlines()
		file_stream.seek(0, 0)

		row_idx = 0
		for line in lines:
			row_idx += 1
			pattern = GetMatchPattern()
			results = pattern.findall(line.decode('utf-8'))
			if len(results) >= 1:
				match_idx = 0
				for lang_str in results:
					match_idx += 1
					str_idx += 1
					str_file_stream.write("(%s_%d_%d)[%d]" % (str_info["FileName"], row_idx, match_idx, str_idx) + lang_str + "\n")
				pass
			pass
		pass
	pass

	str_file_stream.close()
pass

if __name__ == "__main__":
	FindAllStrPath()
	CreateLangStrFile()
pass
