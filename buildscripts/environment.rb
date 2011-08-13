require "buildscripts/paths"
require "buildscripts/project_details"
require 'semver'

namespace :env do

  task :common do
    # version management
    fv = version SemVer.find.to_s
    revision = (!fv[3] || fv[3] == 0) ? (ENV['BUILD_NUMBER'] || Time.now.strftime('%j%H')) : fv[3] #  (day of year 0-265)(hour 00-24)

    ENV['BUILD_VERSION'] = BUILD_VERSION = "#{ SemVer.new(fv[0], fv[1], fv[2]).format "%M.%m.%p" }.#{revision}"
    puts "Assembly Version: #{BUILD_VERSION}."
    puts "##teamcity[buildNumber '#{BUILD_VERSION}']" # tell teamcity our decision

    # .net/mono configuration management
    ENV['FRAMEWORK'] = FRAMEWORK = ENV['FRAMEWORK'] || (Rake::Win32::windows? ? "net40" : "mono28")
    puts "Framework: #{FRAMEWORK}"
  end

  # configure the output directories
  task :configure, [:str] do |_, args|
    ENV['CONFIGURATION'] = CONFIGURATION = args[:str]
    FOLDERS[:binaries] = File.join(FOLDERS[:build], FRAMEWORK, args[:str].downcase)
    CLEAN.include(File.join(FOLDERS[:binaries], "*"))
  end

  task :set_dirs do

    outp = FRAMEWORK + "-" + CONFIGURATION

    FOLDERS[:ew][:out] = File.join(FOLDERS[:src], PROJECTS[:ew][:dir], 'bin', outp)
    CLEAN.include(FOLDERS[:ew][:out])

    FOLDERS[:fs][:out] = File.join(FOLDERS[:src], PROJECTS[:fs][:dir], 'bin', outp)
    CLEAN.include(FOLDERS[:fs][:out])

    FOLDERS[:log][:out] = File.join(FOLDERS[:src], PROJECTS[:log][:dir], 'bin', outp)
    CLEAN.include(FOLDERS[:log][:out])

    FOLDERS[:rem][:out] = File.join(FOLDERS[:src], PROJECTS[:rem][:dir], 'bin', outp)
    CLEAN.include(FOLDERS[:rem][:out])

    FOLDERS[:syn][:out] = File.join(FOLDERS[:src], PROJECTS[:syn][:dir], 'bin', outp)
    CLEAN.include(FOLDERS[:syn][:out])
	
    FOLDERS[:win][:out] = File.join(FOLDERS[:src], PROJECTS[:win][:dir], 'bin', outp)
    CLEAN.include(FOLDERS[:win][:out])

    # for tests
    FOLDERS[:win][:test_out] = File.join(FOLDERS[:src], PROJECTS[:win][:test_dir], 'bin', CONFIGURATION)
    FILES[:win][:test] = File.join(FOLDERS[:win][:test_out], "#{PROJECTS[:win][:test_dir]}.dll")
    CLEAN.include(FOLDERS[:test_out])

  end

  task :dir_tasks do
    all_dirs = []

    [:build, :tools, :tests, :nuget, :nuspec].each do |dir|
      directory FOLDERS[dir]
      all_dirs <<  FOLDERS[dir]
    end

    [:out, :nuspec, :test_out].each do |dir|
      [:ew, :fs, :log, :rem, :syn, :win].each{ |k|
        directory FOLDERS[k][dir]
        all_dirs << FOLDERS[k][dir]
      }
    end

    all_dirs.each do |d|
      Rake::Task[d].invoke
    end
  end

  # DEBUG/RELEASE

  desc "set debug environment variables"
  task :debug => [:common] do
    Rake::Task["env:configure"].invoke('Debug')
    Rake::Task["env:set_dirs"].invoke
    Rake::Task["env:dir_tasks"].invoke
  end

  desc "set release environment variables"
  task :release => [:common] do
    Rake::Task["env:configure"].invoke('Release')
    Rake::Task["env:set_dirs"].invoke
    Rake::Task["env:dir_tasks"].invoke
  end

  # FRAMEWORKS

  desc "set net40 framework"
  task :net40 do
    ENV['FRAMEWORK'] = 'net40'
  end

  desc "set net35 framework"
  task :net35 do
    ENV['FRAMEWORK'] = 'net35'
  end

  desc "set net20 framework"
  task :net20 do
    ENV['FRAMEWORK'] = 'net20'
  end

  desc "set mono28 framework"
  task :mono28 do
    ENV['FRAMEWORK'] = 'mono28'
  end

  desc "set mono30 framework"
  task :net30 do
    ENV['FRAMEWORK'] = 'mono30'
  end

  # ENVIRONMENT VARS FOR PRODUCT RELEASES

  desc "set GA envionment variables"
  task :ga do
    puts "##teamcity[progressMessage 'Setting environment variables for GA']"
    ENV['OFFICIAL_RELEASE'] = OFFICIAL_RELEASE = "4000"
  end

  desc "set release candidate environment variables"
  task :rc, [:number] do |t, args|
    puts "##teamcity[progressMessage 'Setting environment variables for Release Candidate']"
    arg_num = args[:number].to_i
    num = arg_num != 0 ? arg_num : 1
    ENV['OFFICIAL_RELEASE'] = OFFICIAL_RELEASE = "#{3000 + num}"
  end

  desc "set beta-environment variables"
  task :beta, [:number] do |t, args|
    puts "##teamcity[progressMessage 'Setting environment variables for Beta']"
    arg_num = args[:number].to_i
    num = arg_num != 0 ? arg_num : 1
    ENV['OFFICIAL_RELEASE'] = OFFICIAL_RELEASE = "#{2000 + num}"
  end

  desc "set alpha environment variables"
  task :alpha, [:number] do |t, args|
    puts "##teamcity[progressMessage 'Setting environment variables for Alpha']"
    arg_num = args[:number].to_i
    num = arg_num != 0 ? arg_num : 1

    ENV['OFFICIAL_RELEASE'] = OFFICIAL_RELEASE = "#{1000 + num}"
  end
end
