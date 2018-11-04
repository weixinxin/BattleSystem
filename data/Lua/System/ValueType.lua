--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local ValueType = {}

ValueType[int64]		= 11
ValueType[uint64]		= 12

local function GetValueType()	
	local getmetatable = getmetatable
	local ValueType = ValueType

	return function(udata)
		local meta = getmetatable(udata)	

		if meta == nil then
			return 0
		end

		return ValueType[meta] or 0
	end
end

function AddValueType(table, type)
	ValueType[table] = type
end

GetLuaValueType = GetValueType() 