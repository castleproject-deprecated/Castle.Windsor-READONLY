require "rubygems"
require "bundler"
Bundler.setup
$: << './'

require 'albacore'
require 'rake/clean'
require 'semver'

require 'buildscripts/utils'
require 'buildscripts/paths'
require 'buildscripts/project_details'
require 'buildscripts/environment'

# to get the current version of the project, type 'SemVer.find.to_s' in this rake file.

desc 'generate the shared assembly info'
assemblyinfo :assemblyinfo => ["env:release"] do |asm|
  data = commit_data() #hash + date
  asm.product_name = asm.title = PROJECTS[:ew][:title]
  asm.description = PROJECTS[:win][:description] + " #{data[0]} - #{data[1]}"
  asm.company_name = PROJECTS[:win][:company]
  # This is the version number used by framework during build and at runtime to locate, link and load the assemblies. When you add reference to any assembly in your project, it is this version number which gets embedded.
  asm.version = BUILD_VERSION
  # Assembly File Version : This is the version number given to file as in file system. It is displayed by Windows Explorer. Its never used by .NET framework or runtime for referencing.
  asm.file_version = BUILD_VERSION
  asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION}", # disposed as product version in explorer
    :CLSCompliantAttribute => true,
    :AssemblyConfiguration => "#{CONFIGURATION}",
    :Guid => PROJECTS[:win][:guid]
  asm.com_visible = false
  asm.copyright = PROJECTS[:ew][:copyright]
  asm.output_file = File.join(FOLDERS[:src], 'SharedAssemblyInfo.cs')
  asm.namespaces = "System", "System.Reflection", "System.Runtime.InteropServices", "System.Security"
end


desc "build sln file"
msbuild :msbuild do |msb|
  msb.solution   = FILES[:sln]
  msb.properties :Configuration => FRAMEWORK + "-" + CONFIGURATION
  msb.targets    :Clean, :Build
end


task :ew_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:ew][:id])
  copy_files FOLDERS[:ew][:out], "*.{xml,dll,pdb,config}", target
  CLEAN.include(target)
end


task :fs_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:fs][:id])
  copy_files FOLDERS[:fs][:out], "*.{xml,dll,pdb,config}", target
  CLEAN.include(target)
end


task :log_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:log][:id])
  copy_files FOLDERS[:log][:out], "*.{xml,dll,pdb,config}", target
  CLEAN.include(target)
end


task :rem_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:rem][:id])
  copy_files FOLDERS[:rem][:out], "*.{xml,dll,pdb,config}", target
  CLEAN.include(target)
end


task :syn_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:syn][:id])
  copy_files FOLDERS[:syn][:out], "*.{xml,dll,pdb,config}", target
  CLEAN.include(target)
end


task :win_output => [:msbuild] do
  target = File.join(FOLDERS[:binaries], PROJECTS[:win][:id])
  copy_files FOLDERS[:win][:out], "*.{xml,dll,pdb,config}", target
  CLEAN.include(target)
end

task :output => [:ew_output, :fs_output, :log_output, :rem_output, :syn_output, :win_output]
task :nuspecs => [:ew_nuspec, :fs_nuspec, :log_nuspec, :rem_nuspec, :syn_nuspec, :win_nuspec]

desc "Create a nuspec for 'Castle.Facilities.EventWiring'"
nuspec :ew_nuspec do |nuspec|
  nuspec.id = "#{PROJECTS[:ew][:nuget_key]}"
  nuspec.version = BUILD_VERSION
  nuspec.authors = "#{PROJECTS[:ew][:authors]}"
  nuspec.description = "#{PROJECTS[:ew][:description]}"
  nuspec.title = "#{PROJECTS[:ew][:title]}"
  # nuspec.projectUrl = 'http://github.com/haf' # TODO: Set this for nuget generation
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0" # TODO: set this for nuget generation
  nuspec.requireLicenseAcceptance = "false"
  
  nuspec.output_file = FILES[:ew][:nuspec]
  nuspec_copy(:ew, "#{PROJECTS[:ew][:id]}.{dll,pdb,xml}")
end


desc "Create a nuspec for 'Castle.Facilities.FactorySupport'"
nuspec :fs_nuspec do |nuspec|
  nuspec.id = "#{PROJECTS[:fs][:nuget_key]}"
  nuspec.version = BUILD_VERSION
  nuspec.authors = "#{PROJECTS[:fs][:authors]}"
  nuspec.description = "#{PROJECTS[:fs][:description]}"
  nuspec.title = "#{PROJECTS[:fs][:title]}"
  # nuspec.projectUrl = 'http://github.com/haf' # TODO: Set this for nuget generation
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0" # TODO: set this for nuget generation
  nuspec.requireLicenseAcceptance = "false"
  
  nuspec.output_file = FILES[:fs][:nuspec]
  nuspec_copy(:fs, "#{PROJECTS[:fs][:id]}.{dll,pdb,xml}")
end


desc "Create a nuspec for 'Castle.Facilities.Logging'"
nuspec :log_nuspec do |nuspec|
  nuspec.id = "#{PROJECTS[:log][:nuget_key]}"
  nuspec.version = BUILD_VERSION
  nuspec.authors = "#{PROJECTS[:log][:authors]}"
  nuspec.description = "#{PROJECTS[:log][:description]}"
  nuspec.title = "#{PROJECTS[:log][:title]}"
  # nuspec.projectUrl = 'http://github.com/haf' # TODO: Set this for nuget generation
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0" # TODO: set this for nuget generation
  nuspec.requireLicenseAcceptance = "false"
  
  nuspec.output_file = FILES[:log][:nuspec]
  nuspec_copy(:log, "#{PROJECTS[:log][:id]}.{dll,pdb,xml}")
end


desc "Create a nuspec for 'Castle.Facilities.Remoting'"
nuspec :rem_nuspec do |nuspec|
  nuspec.id = "#{PROJECTS[:rem][:nuget_key]}"
  nuspec.version = BUILD_VERSION
  nuspec.authors = "#{PROJECTS[:rem][:authors]}"
  nuspec.description = "#{PROJECTS[:rem][:description]}"
  nuspec.title = "#{PROJECTS[:rem][:title]}"
  # nuspec.projectUrl = 'http://github.com/haf' # TODO: Set this for nuget generation
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0" # TODO: set this for nuget generation
  nuspec.requireLicenseAcceptance = "false"
  
  nuspec.output_file = FILES[:rem][:nuspec]
  nuspec_copy(:rem, "#{PROJECTS[:rem][:id]}.{dll,pdb,xml}")
end


desc "Create a nuspec for 'Castle.Facilities.Synchronize'"
nuspec :syn_nuspec do |nuspec|
  nuspec.id = "#{PROJECTS[:syn][:nuget_key]}"
  nuspec.version = BUILD_VERSION
  nuspec.authors = "#{PROJECTS[:syn][:authors]}"
  nuspec.description = "#{PROJECTS[:syn][:description]}"
  nuspec.title = "#{PROJECTS[:syn][:title]}"
  # nuspec.projectUrl = 'http://github.com/haf' # TODO: Set this for nuget generation
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0" # TODO: set this for nuget generation
  nuspec.requireLicenseAcceptance = "false"
  
  nuspec.output_file = FILES[:syn][:nuspec]
  nuspec_copy(:syn, "#{PROJECTS[:syn][:id]}.{dll,pdb,xml}")
end


desc "Create a nuspec for 'Castle.Windsor'"
nuspec :win_nuspec do |nuspec|
  nuspec.id = "#{PROJECTS[:win][:nuget_key]}"
  nuspec.version = BUILD_VERSION
  nuspec.authors = "#{PROJECTS[:win][:authors]}"
  nuspec.description = "#{PROJECTS[:win][:description]}"
  nuspec.title = "#{PROJECTS[:win][:title]}"
  # nuspec.projectUrl = 'http://github.com/haf' # TODO: Set this for nuget generation
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0" # TODO: set this for nuget generation
  nuspec.requireLicenseAcceptance = "false"
  
  nuspec.output_file = FILES[:win][:nuspec]
  nuspec_copy(:win, "#{PROJECTS[:win][:id]}.{dll,pdb,xml}")
end

task :nugets => [:"env:release", :nuspecs, :ew_nuget, :fs_nuget, :log_nuget, :rem_nuget, :syn_nuget, :win_nuget]

desc "nuget pack 'Castle.Facilities.EventWiring'"
nugetpack :ew_nuget do |nuget|
   nuget.command     = "#{COMMANDS[:nuget]}"
   nuget.nuspec      = "#{FILES[:ew][:nuspec]}"
   # nuget.base_folder = "."
   nuget.output      = "#{FOLDERS[:nuget]}"
end


desc "nuget pack 'Castle.Facilities.FactorySupport'"
nugetpack :fs_nuget do |nuget|
   nuget.command     = "#{COMMANDS[:nuget]}"
   nuget.nuspec      = "#{FILES[:fs][:nuspec]}"
   # nuget.base_folder = "."
   nuget.output      = "#{FOLDERS[:nuget]}"
end


desc "nuget pack 'Castle.Facilities.Logging'"
nugetpack :log_nuget do |nuget|
   nuget.command     = "#{COMMANDS[:nuget]}"
   nuget.nuspec      = "#{FILES[:log][:nuspec]}"
   # nuget.base_folder = "."
   nuget.output      = "#{FOLDERS[:nuget]}"
end


desc "nuget pack 'Castle.Facilities.Remoting'"
nugetpack :rem_nuget do |nuget|
   nuget.command     = "#{COMMANDS[:nuget]}"
   nuget.nuspec      = "#{FILES[:rem][:nuspec]}"
   # nuget.base_folder = "."
   nuget.output      = "#{FOLDERS[:nuget]}"
end


desc "nuget pack 'Castle.Facilities.Synchronize'"
nugetpack :syn_nuget do |nuget|
   nuget.command     = "#{COMMANDS[:nuget]}"
   nuget.nuspec      = "#{FILES[:syn][:nuspec]}"
   # nuget.base_folder = "."
   nuget.output      = "#{FOLDERS[:nuget]}"
end


desc "nuget pack 'Castle.Windsor'"
nugetpack :win_nuget do |nuget|
   nuget.command     = "#{COMMANDS[:nuget]}"
   nuget.nuspec      = "#{FILES[:win][:nuspec]}"
   # nuget.base_folder = "."
   nuget.output      = "#{FOLDERS[:nuget]}"
end

task :publish => [:"env:release", :ew_nuget_push, :fs_nuget_push, :log_nuget_push, :rem_nuget_push, :syn_nuget_push, :win_nuget_push]

desc "publishes (pushes) the nuget package 'Castle.Facilities.EventWiring'"
nugetpush :ew_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:ew][:nuget_key] + "." + BUILD_VERSION + '.nupkg')}"
# nuget.apikey = "...."
  nuget.source = URIS[:nuget_offical]
  nuget.create_only = false
end


desc "publishes (pushes) the nuget package 'Castle.Facilities.FactorySupport'"
nugetpush :fs_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:fs][:nuget_key] + "." + BUILD_VERSION + '.nupkg')}"
# nuget.apikey = "...."
  nuget.source = URIS[:nuget_offical]
  nuget.create_only = false
end


desc "publishes (pushes) the nuget package 'Castle.Facilities.Logging'"
nugetpush :log_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:log][:nuget_key] + "." + BUILD_VERSION + '.nupkg')}"
# nuget.apikey = "...."
  nuget.source = URIS[:nuget_offical]
  nuget.create_only = false
end


desc "publishes (pushes) the nuget package 'Castle.Facilities.Remoting'"
nugetpush :rem_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:rem][:nuget_key] + "." + BUILD_VERSION + '.nupkg')}"
# nuget.apikey = "...."
  nuget.source = URIS[:nuget_offical]
  nuget.create_only = false
end


desc "publishes (pushes) the nuget package 'Castle.Facilities.Synchronize'"
nugetpush :syn_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:syn][:nuget_key] + "." + BUILD_VERSION + '.nupkg')}"
# nuget.apikey = "...."
  nuget.source = URIS[:nuget_offical]
  nuget.create_only = false
end


desc "publishes (pushes) the nuget package 'Castle.Windsor'"
nugetpush :win_nuget_push do |nuget|
  nuget.command = "#{COMMANDS[:nuget]}"
  nuget.package = "#{File.join(FOLDERS[:nuget], PROJECTS[:win][:nuget_key] + "." + BUILD_VERSION + '.nupkg')}"
# nuget.apikey = "...."
  nuget.source = URIS[:nuget_offical]
  nuget.create_only = false
end

task :default  => ["env:release", "assemblyinfo", "msbuild", "output", "nugets"]