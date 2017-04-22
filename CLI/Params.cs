using CliParse;

namespace CLI
{
    [ParsableClass("Example CLI Parsable", "This is a description.", FooterText = "This is the footer text.")]
    public class Params : Parsable
    {
        [ParsableArgument("config", ShortName = 'c', ImpliedPosition = 1, Required = true,
            DefaultValue = "bundle.config.json", Example = "-c 'path_to_config.json'")]
        public string Config { get; set; }

        [ParsableArgument("bumpBuild", ShortName = 'b', DefaultValue = false)]
        public bool BumpBuild { get; set; }

        [ParsableArgument("bumpSprint", ShortName = 's', DefaultValue = false)]
        public bool BumpSprint { get; set; }
    }
}