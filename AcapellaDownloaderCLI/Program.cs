using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Apparently this will make the code cross-platform compatible
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

using AcapellaDownloader;
using System.Net;

namespace AcapellaDownloaderCLI
{
    class Program
    {
        static int Main(string[] args)
        {

            /*TODO
             * Program will require the following in order to work
             * Input Text 
             * Voice
             * Output file
             * 
             * Misc
             * Voice List to see what can be downloaded.
             */

            //using System's beta tools! Woah!
            var rootCommand = new RootCommand
            {
                new Option(new string[2] {"-i", "--input-Text" },"The text that will be converted to speech")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },

                new Option(new string[2] {"-v", "--voice" }, "The voice that will read the text")
                {
                    Argument = new Argument<string>()
                    , Required = true
                },
                new Option(new string[2] {"-o", "--output" }, "The file location it will be saved into. The file must end with .mp3")
                {
                    Argument = new Argument<string>(),
                    Required = true
                },
                new Command("--voiceList", "A list of available voices to pull from.")
                {
                    Handler = CommandHandler.Create<Task>( x =>
                    {
                        Voices.Load();
                        int i = 0;
                        foreach(var voice in Voices.VoiceList)
                        {
                            i++;
                            Console.WriteLine($"{voice.Name} - {voice.Lang}");
                        }
                    })
                }

            };

            rootCommand.Description = "An Attempt to perform an CLI using AcapellaDownloader Library";

            rootCommand.Handler = CommandHandler.Create<string, string, string>((inputText, voice, output) =>
            {

                if (!output.EndsWith(".mp3"))
                {
                    Console.WriteLine("The file must end with .mp3");
                    return;
                }

                string link = "";

                try
                {
                    Voices.Load();
                    var v = Voices.VoiceList.First(n => n.Name.Equals(voice));
                    link = Utils.Parse(inputText, v.VoiceFile);
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine($"That voice does not exist. Voice {voice} is not in the list. ");
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    return;
                }



                if (link == "")
                {
                    Console.WriteLine("Can't download. Maybe this voice is paid.");
                }
                using (var web = new WebClient())
                {
                    web.DownloadFile(link, output);
                    Console.WriteLine($"File downloaded at {output}");
                }


            });


            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
