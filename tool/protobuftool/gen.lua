
--要自动进行消息号分配的协议文件
local config = 
{
	--文件根目录
	root = "./",

	--普通规则
	--		文件名
	filelist = 
	{
		"login",
		"user",
		"battle",
	},

	--输出文件规则
	out_file_rule = 
	{
		{ login = "opcode_role_client" },
		{ user = "opcode_role_client" },
		{ battle = "opcode_role_client" },
	},

	--消息号规则
	--		文件名(包含虚拟文件名) => 起始消息号
	opcode = 
	{
		["login"] = { start_opcode = 5001, end_opcode = 7000 },
		["user"] = { start_opcode = 7001, end_opcode = 10000 },
		["battle"] = { start_opcode = 9000, end_opcode = 10000 },
	},

	--特殊规则
	special = 
	{
		--协议规则
		--		协议名 => 虚拟文件名
		protocol = 
		{
		},

		--文件规则
		--		文件名 => 虚拟文件名
		filename = 
		{
		},
	},

	--忽略错误
	ignore_error = true,
}



--------------------------------------------------------------------------------
--------------------------------------------------------------------------------
local name_rule = 
{
	--只处理指定末尾
	suffix = 
	{
		"Req",
		"ReqResp",
		"Resp",
		"Sub",
		"Pub",
		"Notify",
		"Ntf",
	},

	--过滤生成提取规则(必须配对)
	filter = 
	{
		["Req"] = { response = {"Resp", "ReqResp"}, is_request = true },
		["Sub"] = { response = {"Pub"} },
		["Notify"] = { response = nil },
		["Ntf"] = { response = nil },
	},
}

local mgr = 
{
	--所有协议(符合指定末尾规则)
	--	协议名 => 信息
	--				{
	--					idx			定义顺序
	--					name		名称(不带末尾)
	--					filename	文件名
	--				}
	all = {},

	--所有名称(不带末尾)
	--	名称(不带末尾) => true
	all_name = {},

	--协议集(满足提取规则)
	--	协议名 => 协议信息
	--				{
	--					idx				定义顺序
	--					filename		文件名
	--					raw_filename	原始文件名
	--					name			协议名
	--					opcode			协议号
	--					is_request		是否是请求
	--					response		响应协议信息
	--									{
	--										name	协议名
	--										opcode	协议号
	--									}
	--				}
	filter = {},

	--最终结果(按协议名排序，并生成opcode)
	--	文件名 => 协议集
	--				索引 => 协议信息(同上) - 对filter信息对象的引用
	result = nil,

	--协议定义的先后顺序(1 - N)
	idx = 0,
}



local function check_suffix(name)
	for k, v in pairs(name_rule.suffix) do
		local str = name:match("(%w+)" .. v .. "$")
		if str then
			return true, str
		end
	end
	return false
end

local function get_real_filename(name, filename)
	local str = config.special.protocol[name]
	if str then
		return str
	end

	str = config.special.filename[filename]
	if str then
		return str
	end

	return filename
end

local function parse_data(filename, data)
	local str = data:match("package (%w+);")
	if str ~= filename then
		print(string.format("filename != package name, filename:[%s], package name:[%s]", filename, str))
		return false
	end

	for name in data:gmatch("message%s(%w+)") do
		local ok, str = check_suffix(name)
		if ok then
			mgr.idx = mgr.idx + 1
			idx = mgr.idx

			mgr.all_name[str] = true
			mgr.all[name] = 
			{
				idx = idx,
				name = str,
				filename = get_real_filename(name, filename),
				raw_filename = filename,
			}
		end
	end

	return true
end

local function parse_all()
	for k, v in pairs(config.filelist) do
		local path = config.root .. v .. ".proto"
		local f = io.open(path, "r")
		if not f then
			print(string.format("open file failed, path:[%s]", path))
			return false
		end
		local data = f:read("*a")
		f:close()

		if not parse_data(v, data) then
			print(string.format("parse data failed. path:[%s]", path))
			return false
		end
	end
	return true
end

local function add_filter_info(name)
	local num = 0
	for k, v in pairs(name_rule.filter) do
		local str = name .. k
		local info = mgr.all[str]
		if info then
			local res_str = nil
			local response = nil
			local is_request = false
			if v.response then
				if v.is_request then
					is_request = true
				end

				for _, one in pairs(v.response) do
					local temp = name .. one
					if mgr.all[temp] then
						res_str = temp
						break
					end
				end

				if not res_str then
					print(string.format("[%s] is pair. not find other part. filter:[%s]", name, k))
					return false
				end

				response = 
				{
					name = res_str,
					opcode = nil,
				}
			end

			mgr.filter[str] = 
			{
				idx = info.idx,
				filename = info.filename,
				raw_filename = info.raw_filename,
				name = str,
				opcode = 0,
				is_request = is_request,
				response = response,
			}
			num = num + 1
		end
	end

	return num > 0
end

local function process_filter()
	for k, v in pairs(mgr.all_name) do
		if not add_filter_info(k) then
			print(string.format("filter failed, no has suffix name:[%s]", k))
			if not config.ignore_error then
				return false
			end
		end
	end
	return true
end

local function alloc_one_opcode(filename, name, info)
	local id = info.now_opcode
	if not id then
		id = info.start_opcode
		info.now_opcode = id
	end

	if info.now_opcode > info.end_opcode then
		print(string.format("alloc opcode failed, [%s] opcode need [%s - %s], " .. 
							"but now will for [%s] alloc [%s], error.", 
							filename, info.start_opcode, info.end_opcode, name, info.now_opcode))
		return nil
	end

	info.now_opcode = info.now_opcode + 1
	return id
end

local function alloc_opcode(filename, lt)
	local info = config.opcode[filename]
	if not info then
		print(string.format("not find [%s] opcode.", filename))
		return false
	end

	for k, v in pairs(lt) do
		local id = alloc_one_opcode(filename, v.name, info)
		if not id then
			return false
		end

		v.opcode = id
		if v.response then
			id = alloc_one_opcode(filename, v.response.name, info)
			if not id then
				return false
			end

			v.response.opcode = id
		end
	end

	return true
end

local function make_result()
	local lt = {}
	for k, v in pairs(mgr.filter) do
		local temp = lt[v.filename]
		if not temp then
			temp = {}
			lt[v.filename] = temp
		end
		table.insert(temp, v)
	end

	for k, v in pairs(lt) do
		table.sort(v, function(a, b) return a.idx < b.idx end)
	end

	for k, v in pairs(lt) do
		if not alloc_opcode(k, v) then
			print(string.format("alloc [%s] opcode failed.", k))
			return false
		end
	end

	mgr.result = lt
	return true
end

local function write_client_file(result)
	local path = config.root .. "protocols.lua"
	local f = io.open(path, "w")
	if not f then
		print(string.format("open write client file failed, path:[%s]", path))
		return false
	end

	f:write("local protocols = {}\n")
	local name_by_code = {}
	local code_by_name = {}
	for _, o in pairs(config.out_file_rule) do
		for k, v in pairs(o) do
			local lt = result[k]
			if lt then
				for _, info in pairs(lt) do
					table.insert(name_by_code, 
							string.format('protocols[%s] = "%s.%s"\n', 
							tostring(info.opcode), info.raw_filename, info.name))
					table.insert(code_by_name, 
							string.format('protocols["%s.%s"] = %s\n', 
							info.raw_filename, info.name, tostring(info.opcode)))
					if info.response then
						table.insert(name_by_code, 
							string.format('protocols[%s] = "%s.%s"\n', 
							tostring(info.response.opcode), info.raw_filename, info.response.name))
						table.insert(code_by_name, 
							string.format('protocols["%s.%s"] = %s\n', 
							info.raw_filename, info.response.name, tostring(info.response.opcode)))
					end
				end
			end
		end
	end

	f:write('protocols[1] = "login.PingPong"\n')
	f:write(table.concat(name_by_code))
	f:write("\n")
	f:write('protocols["login.PingPong"] = 1\n')
	f:write(table.concat(code_by_name))
	f:write("\n")
	f:write("return protocols")
	f:close()
	return true
end

local function write_server_file(result)
	local all_set = {}
	local all_request_set = {}
	local all_response_set = {}
	for _, o in pairs(config.out_file_rule) do
		for k, v in pairs(o) do
			local name_of_code = {}
			all_set[k] = name_of_code

			local lt = result[k]
			if lt then
				for _, info in pairs(lt) do
					table.insert(name_of_code, 
							string.format('["%s.%s"] = %s,\n', 
								info.raw_filename, info.name, tostring(info.opcode)))
					if info.response then
						table.insert(name_of_code, 
							string.format('["%s.%s"] = %s,\n', 
								info.raw_filename, info.response.name, tostring(info.response.opcode)))

						table.insert(all_request_set, 
							string.format(
								'[%s] = { func_name = "%s", msgtype = %s, is_request = %s },\n', 
								tostring(info.opcode), info.name, 
								tostring(info.response.opcode), tostring(info.is_request)))

						table.insert(all_response_set, 
							string.format('[%s] = { is_response = true, is_request = %s },\n', 
								tostring(info.response.opcode), tostring(info.is_request)))
					end
				end
			end
		end
	end

	local file_set = {}
	local all_file = {}
	for _, o in pairs(config.out_file_rule) do
		for k, v in pairs(o) do
			local f = file_set[v]
			if not f then
				local path = config.root .. v .. ".lua"
				f = io.open(path, "w")
				if not f then
					print(string.format("open write server file failed, path:[%s]", path))
					return false
				end
				file_set[v] = f
				f:write("local _mt = \n{\n\n\n\n")
			end
		end
	end

	for _, o in pairs(config.out_file_rule) do
		for k, v in pairs(o) do
			assert(not all_file[k])
			all_file[k] = file_set[v]
		end
	end

	for k, v in pairs(all_set) do
		local f = all_file[k]
		f:write(table.concat(v))
		f:write("\n")
	end

	for k, v in pairs(file_set) do
		v:write("\n\n}\n\n\n\nreturn _mt")
		v:close()
	end
	file_set = nil
	all_file = nil

	local path = config.root .. "opcode_request_response.lua"
	local f = io.open(path, "w")
	if not f then
		print(string.format("open response failed, path:[%s]", path))
		return false
	end

	f:write("local _mt = {}\n\n\n\n")

	f:write("local request = \n{\n\n\n\n")
	f:write(table.concat(all_request_set))
	f:write("\n\n\n}\n\n\n\n")

	f:write("local response = \n{\n\n\n\n")
	f:write(table.concat(all_response_set))
	f:write("\n\n\n}\n\n\n\n")


	f:write(
[[--获取请求消息号对应的函数名、回应消息号、是否为请求
--@param {int} msgtype 消息号
--@return {string}
function _mt.getRequestMsgInfo(msgtype)
	local info = request[msgtype]
	if not info then
		return nil
	end
	return info.func_name, info.msgtype, info.is_request
end


--检测指定消息号是否为回应、是否为请求触发
--@param {int} msgtype 消息号
--@return {boolean} {boolean}
function _mt.checkResponseMsgInfo(msgtype)
	local info = response[msgtype]
	if not info then
		return
	end
	return info.is_response, info.is_request
end]])

	f:write("\n\n\n\nreturn _mt")
	f:close()
	return true
end

local function write_result()
	local result = mgr.result
	if not write_client_file(result) then
		print("write client file failed.")
		return false
	end

	if not write_server_file(result) then
		print("write server file failed.")
		return false
	end
	return true
end


local function start()
	if not parse_all() then
		print("parse failed.")
		return
	end

	if not process_filter() then
		print("filter failed.")
		return
	end

	make_result()

	if not write_result() then
		print("write result failed.")
		return
	end
end



start()
--os.execute("pause")
