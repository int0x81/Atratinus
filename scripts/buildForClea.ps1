$platform = "win-x64" # "osx.10.11-x64"
$outDir = "C:\Users\finnf\Desktop\Atratinus"

dotnet publish -r $platform -c Release --output $outDir -p:PublishSingleFile=true