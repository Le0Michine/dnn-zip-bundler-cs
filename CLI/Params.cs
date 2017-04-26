using CliParse;

namespace CLI
{
    [ParsableClass("DNN zip packager", "Creates zip packages for DNN modules using provided config files.", FooterText = "More detailed information available on the github page\nhttps://github.com/Le0Michine/dnn-zip-bundler-cs")]
    public class Params : Parsable
    {
        [ParsableArgument("config", ShortName = 'c', ImpliedPosition = 1, Required = true, Description = "path to json config file")]
        public string Config { get; set; }

        [ParsableArgument("bumpBuild", ShortName = 'b', DefaultValue = false, Description = "increment build number")]
        public bool BumpBuild { get; set; }

        [ParsableArgument("bumpSprint", ShortName = 's', DefaultValue = false, Description = "increment sprint number and reset build number to 1")]
        public bool BumpSprint { get; set; }

        [ParsableArgument("targetVersion", ShortName = 'v', Description = "set specific version")]
        public string TargetVersion { get; set; }
    }
}