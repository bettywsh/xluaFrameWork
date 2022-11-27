echo "gen lua file"
lua gen.lua

echo "gen pb file"
protoc.exe --descriptor_set_out=battle.pb battle.proto
protoc.exe --descriptor_set_out=user.pb user.proto
protoc.exe --descriptor_set_out=login.pb login.proto
protoc.exe --descriptor_set_out=netmsg.pb netmsg.proto

echo "finished"

Pause
