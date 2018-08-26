#!/usr/bin/python
#coding=utf-8

import os,sys
import re

os.chdir(os.path.abspath(sys.path[0]))
reload(sys)
sys.setdefaultencoding('utf-8')

ExportFilePath = "../Socket/MsgCode.cs"

RemarkPattern = None
MsgPattern = None

MsgName_List = []
MsgCode_List = []
MsgSceneId_list = {}
LastSceneId = None

def GetRemarkPattern():
	global RemarkPattern
	if not RemarkPattern:
		RemarkPattern = re.compile(r'\s*//\s*\[(\S*)\]\s*')
	pass
	return RemarkPattern
pass

def GetMsgPattern():
	global MsgPattern
	if not MsgPattern:
		MsgPattern = re.compile(r'\s*message\s*(([A-Z][A-Z])([a-zA-Z]*))\s*')
	pass
	return MsgPattern
pass

def CheckLine(file_name, line):
	# 先查找备注，记录需要发往的场景ID
	global LastSceneId
	pattern = GetRemarkPattern()
	results = pattern.findall(line.decode('utf-8'))
	if len(results) >= 1:
		LastSceneId = int(results[0])
		return
	pass

	# 不匹配再查找协议
	pattern = GetMsgPattern()
	results = pattern.findall(line.decode('utf-8'))
	if len(results) >= 1:
		# ST为自定义数据体开头标志
		if results[0][1] != "ST":
			print file_name, results
			msg_name = results[0][1] + results[0][2]
			msg_code = results[0][1] + "_" + results[0][2]
			MsgName_List.append(msg_name)
			MsgCode_List.append(msg_code)

			# 说明该协议写了需要发往场景ID的注释
			if LastSceneId != None:
				MsgSceneId_list[msg_code] = LastSceneId
				LastSceneId = None
			pass
		pass
	pass
pass

def FindAllMsgName():
	for root_path, dirs, files in os.walk("."):
		for file in files:
			file_path = "%s%s%s" % (root_path, os.sep, file)
			str_list = os.path.splitext(file)
			file_name = str_list[0]
			file_format = str_list[1]

			if file_format == ".proto":
				file_stream = open(file_path, 'r')
				lines = file_stream.readlines()
				for line in lines:
					CheckLine(file_name, line)
				pass
			pass
		pass
	pass
pass

def ExportMsgCode():
	stream = open(ExportFilePath, 'w+')

	line = "\
using Common.Protobuf;\n\
using Google.Protobuf;\n\
using System;\n\
using System.Collections.Generic;\n\
\n\
class MsgCode\n\
{\n\
"
	stream.write(line)

	# 协议号分配
	cur_msg_id = 10000
	for msg_code in MsgCode_List:
		cur_msg_id = cur_msg_id + 1
		stream.write("\tpublic const short %s = %d;\n" % (msg_code, cur_msg_id))

		# 更改为协议ID映射到场景ID
		if MsgSceneId_list.has_key(msg_code):
			MsgSceneId_list[cur_msg_id] = MsgSceneId_list.pop(msg_code)
		pass
	pass

	# 协议号对应的协议解析类
	line = "\tpublic static Dictionary<short, MessageParser> ProtocolParser = new Dictionary<short, MessageParser>() {\n"
	stream.write("\n" + line)

	for i in xrange(0, len(MsgCode_List) - 1):
		stream.write("\t\t{MsgCode.%s, %s.Parser},\n" % (MsgCode_List[i], MsgName_List[i]))
	pass

	line = "\t};\n"
	stream.write(line)

	# 协议对应的协议号
	line = "\tpublic static Dictionary<Type, short> ProtocolMap = new Dictionary<Type, short>(){\n"
	stream.write("\n" + line)

	for i in xrange(0, len(MsgCode_List) - 1):
		stream.write("\t\t{typeof(%s), MsgCode.%s},\n" % (MsgName_List[i], MsgCode_List[i]))
	pass

	line = "\t};\n"
	stream.write(line)

	# 生成路由协议号转发到指定场景ID映射
	line = "\tpublic static Dictionary<short, int> ProtocolSceneId = new Dictionary<short, int>() {\n"
	stream.write("\n" + line)

	for msg_id, scene_id in MsgSceneId_list.items():
		stream.write("\t\t{%d, %d},\n" % (msg_id, scene_id))
	pass

	line = "\t};\n"
	stream.write(line)

	line = "};\n"
	stream.write(line)

	stream.close()
pass

if __name__ == "__main__":
	FindAllMsgName()
	ExportMsgCode()
pass
