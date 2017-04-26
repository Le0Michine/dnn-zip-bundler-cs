# dnn-zip-bundler-cs

CLI app to automate DNN packages creation

## Installation and usage

Download latest dnnBundler.exe from [releases tab](https://github.com/Le0Michine/dnn-zip-bundler-cs/releases). To start packaging run the following cmd-let in windows console:

    dnnbundler.exe config.json
    
`config.json` is a configuration file describing zip package you are going to create. Detailed schema of config file can be found by the links:
    
* [BundleConfigurationSchema](https://github.com/Le0Michine/dnn-zip-bundler-cs/blob/master/CLI/BundlerConfigurationSchema.json)
* [ZipPackageSchema](https://github.com/Le0Michine/dnn-zip-bundler-cs/blob/master/Zipper/ConfigurationSchema.json)
    
An example of configuration is provided below:


    {
        "packages": [
            {
                "name": "out.[PACKAGE_VERSION].zip", // [PACKAGE_VERSION] is a placeholder for package version which will be taken from manifest file
                "entries": [
                    "path_to_file",
                    "path_to_directory",             // real path in file system to file or directory
                    {
                        "type": "file",              // type of entry, if absent will be treated as 'file'
                        "name": "test.json",         // real path in file system to file or directory
                        "path": "new_path_in_zip"    // optional
                    },
                    {
                        "type": "zip",               // nested zip archive
                        "name": "test.zip",          // name of nested zip archive, can include directories. 'path' property is being ignored for this kind of entries
                        "ignoreEntries": [ ... ],    // local array of entries to ignore
                        "entries": [                 // array of entries for nested zip file, same format as above
                            "file",
                            "dir",
                            "..."
                        ]
                    }
                ],
                "ignoreEntries": [
                    ".DS_Store",                     // entries which should be ignored, optional
                    "*.json"
                ]
            }
        ],
        "manifests": [
            "path_to_dnn_manifest"                   // dnn manifest file, required parameter
        ]
    }
    
## Building from sources

After checking out the repo, restore nuget packages and just run build. As a result you should get an `.exe` file in `CLI/bin/<target>/` folder.

## Contributing

Bug reports and pull requests are welcome on GitHub at https://github.com/Le0Michine/dnn-zip-bundler-cs. This project is intended to be a safe, welcoming space for collaboration, and contributors are expected to adhere to the [Contributor Covenant](http://contributor-covenant.org) code of conduct.


## License

The utility is available as open source under the terms of the [MIT License](http://opensource.org/licenses/MIT).
