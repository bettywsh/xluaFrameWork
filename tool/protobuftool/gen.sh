#!/bin/sh
lua gen.lua
protoc --descriptor_set_out=user.pb user.proto 
protoc --descriptor_set_out=battle.pb battle.proto 
protoc --descriptor_set_out=login.pb login.proto 
protoc --descriptor_set_out=netmsg.pb netmsg.proto

