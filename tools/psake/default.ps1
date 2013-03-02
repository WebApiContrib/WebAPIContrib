Properties {
	$base_dir = resolve-path .\..\..\
	$packages_dir = "$base_dir\packages"
	$build_artifacts_dir = "$base_dir\build"
	$solution_name = "$base_dir\WebApiContrib.sln"
	$nunit_runner = "$packages_dir\NUnit.Runners.2.6.0.12051\tools"
	$nunit_build_destination = "$build_artifacts_dir\tools\nunit"
	$nunitConsole = "$nunit_build_destination\nunit-console.exe"
	$nuget_exe = "$base_dir\.nuget\Nuget.exe"
}

Task Default -Depends BuildWebApiContrib, PrepareForTest, RunUnitTests, NuGetBuild

Task BuildWebApiContrib -Depends Clean, Build

Task Clean {
	Exec { msbuild $solution_name /v:Quiet /t:Clean /p:Configuration=Release }
}

Task Build -depends Clean {
	Exec { msbuild $solution_name /v:Quiet /t:Build /p:Configuration=Release /p:OutDir=$build_artifacts_dir\ } 
}

Task NuGetBuild {
	& $nuget_exe pack "$base_dir/src/WebAPIContrib/WebAPIContrib.csproj" -Build -OutputDirectory $build_artifacts_dir -Verbose -Properties Configuration=Release
	& $nuget_exe pack "$base_dir/src/WebAPIContrib.Testing/WebAPIContrib.Testing.csproj" -Build -OutputDirectory $build_artifacts_dir -Verbose -Properties Configuration=Release
}

Task PrepareForTest {
	$tools_folder = "$build_artifacts_dir\tools"
	if (Test-Path $tools_folder) {
		Remove-Item $tools_folder -Force -Recurse
	}
	Copy-Item "$nunit_runner\*" $nunit_build_destination
	Copy-Item "$nunit_runner\lib\*" "$nunit_build_destination\lib"
}

Task RunUnitTests -depends PrepareForTest, Build {
	$test_result = "$build_artifacts_dir\UnitTestsResult.xml"
	
	& "$nunitConsole" "$build_artifacts_dir\WebApiContribTests.dll" /nologo /nodots /framework:net-4.0 "/xml=$test_result"
	
	if ($lastexitcode -gt 0)
	{
		throw "{0} unit tests failed. See {1} for a simple summary." -f $lastexitcode, $test_result
	}
	if ($lastexitcode -lt 0)
	{
		throw "unit test run was terminated by a fatal error. See {0} for a simple summary." -f $test_result
	}
}
