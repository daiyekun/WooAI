
#### 在Dev.WooAI.EntityFreworkCore 打开命令终端执行下面代码

## 1、首先执行 dotnet ef --version  开始是否安装 dotnet ef tool 如果没有执行下面代码

dotnet tool install --global dotnet-ef 

## 执行迁移命令

dotnet ef migrations add Initial --project "Dev.WooAI.EntityFreworkCore.csproj" --startup-project "../Dev.WooAI.HttpApi/Dev.WooAI.HttpApi.csproj" --context Dev.WooAI.EntityFreworkCore.WooAiDbContext --configuration Debug --output-dir Migrations