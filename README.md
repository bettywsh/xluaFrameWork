# 资源打包
buildconfig文件夹下配置文件夹打类型
1. 文件夹打一个AB
2. 文件夹每个文件打一个AB
3. 文件夹子文件夹单独打一个AB

# 资源加载
资源加载分3种加载方式
1. 全局资源
2. 场景资源
3. 界面资源

# UI
prefab命名规范 #txt,img_name 表示拿到text组件存到txt_name,Image组件存到img_name，#表示要拿组件,##表示跳过改节点其子节点
varprefab脚本会拿到#开头的引用，界面加载后会根据组件缩写获取对应组件

# 事件
ui的事件系统会自动注册和删除， 只要写个方法前缀带事件特殊标记就能找到对应事件

# 网络
使用et7.2网络库，支持tcp websorket kcp 

# protobuf
使用lua-protobuf解析protobuf给lua, c#使用protobuf-net解析protobuf
