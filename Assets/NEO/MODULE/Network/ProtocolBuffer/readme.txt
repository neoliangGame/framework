这是google开发的的数据结构化开源工具

可以对数据进行序列化成二进制和反序列化成结构数据

并且比起传统的XML和json，速度更快，内容更小

并且不失通用性

Google官网：https://developers.google.cn/protocol-buffers/docs/csharptutorial

GitHub地址：https://github.com/protocolbuffers/protobuf

使用简述：

1。下载结构代码编辑工具：protoc.exe
https://github.com/protocolbuffers/protobuf/releases
官网和一些其他网站已经有很多使用教程

把.proto结构数据编译出的.cs文件放到项目中即可使用

2。下载运行时项目，并且编译运行时动态库：
https://github.com/protocolbuffers/protobuf/releases
官网和一些其他网上已经有很多使用教程

把动态库放到项目：Assets/Plugins/