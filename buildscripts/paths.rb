root_folder = File.expand_path("#{File.dirname(__FILE__)}/..")
require "buildscripts/project_details"

# The folders array denoting where to place build artifacts

folders = {
  :root => root_folder,
  :src => "src",
  :build => "build",
  :binaries => "placeholder - environment.rb sets this depending on target",
  :tools => "tools",
  :tests => "build/tests",
  :nuget => "build/nuget",
  :nuspec => "build/nuspec"
}

FOLDERS = folders.merge({

  :ew => {
      :test_dir => '',
      :nuspec => "#{File.join(folders[:nuspec], PROJECTS[:ew][:nuget_key])}",
      :out => 'placeholder - environment.rb will sets this',
      :test_out => 'placeholder - environment.rb sets this'
  },
  
  :fs => {
      :test_dir => '',
      :nuspec => "#{File.join(folders[:nuspec], PROJECTS[:fs][:nuget_key])}",
      :out => 'placeholder - environment.rb will sets this',
      :test_out => 'placeholder - environment.rb sets this'
  },
  
  :log => {
      :test_dir => '',
      :nuspec => "#{File.join(folders[:nuspec], PROJECTS[:log][:nuget_key])}",
      :out => 'placeholder - environment.rb will sets this',
      :test_out => 'placeholder - environment.rb sets this'
  },
  
  :rem => {
      :test_dir => '',
      :nuspec => "#{File.join(folders[:nuspec], PROJECTS[:rem][:nuget_key])}",
      :out => 'placeholder - environment.rb will sets this',
      :test_out => 'placeholder - environment.rb sets this'
  },
  
  :syn => {
      :test_dir => '',
      :nuspec => "#{File.join(folders[:nuspec], PROJECTS[:syn][:nuget_key])}",
      :out => 'placeholder - environment.rb will sets this',
      :test_out => 'placeholder - environment.rb sets this'
  },
  
  :win => {
      :test_dir => 'Castle.Windsor.Tests',
      :nuspec => "#{File.join(folders[:nuspec], PROJECTS[:win][:nuget_key])}",
      :out => 'placeholder - environment.rb will sets this',
      :test_out => 'placeholder - environment.rb sets this'
  },
  
})

FILES = {
  :sln => "Castle.Windsor.sln",
  
  :ew => {
    :nuspec => File.join(FOLDERS[:ew][:nuspec], "#{PROJECTS[:ew][:nuget_key]}.nuspec")
  },
  
  :fs => {
    :nuspec => File.join(FOLDERS[:fs][:nuspec], "#{PROJECTS[:fs][:nuget_key]}.nuspec")
  },
  
  :log => {
    :nuspec => File.join(FOLDERS[:log][:nuspec], "#{PROJECTS[:log][:nuget_key]}.nuspec")
  },
  
  :rem => {
    :nuspec => File.join(FOLDERS[:rem][:nuspec], "#{PROJECTS[:rem][:nuget_key]}.nuspec")
  },
  
  :syn => {
    :nuspec => File.join(FOLDERS[:syn][:nuspec], "#{PROJECTS[:syn][:nuget_key]}.nuspec")
  },
  
  :win => {
    :nuspec => File.join(FOLDERS[:win][:nuspec], "#{PROJECTS[:win][:nuget_key]}.nuspec")
  },
  
}

COMMANDS = {
  :nuget => File.join(FOLDERS[:tools], "NuGet.exe"),
  :ilmerge => File.join(FOLDERS[:tools], "ILMerge.exe")
  # nunit etc
}

URIS = {
  :nuget_offical => "http://packages.nuget.org/v1/",
  :nuget_symbolsource => "http://nuget.gw.symbolsource.org/Public/Nuget"
}