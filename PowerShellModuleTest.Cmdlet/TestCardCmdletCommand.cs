using Newtonsoft.Json;
using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

//see https://www.terrybutler.co.uk/2021/08/12/creating-powershell-module-csharp/
//for some reason this file is not showing
namespace PowerShellModuleTest.Cmdlet
{
    [Cmdlet(VerbsDiagnostic.Test, "CardCmdlet")]
    [OutputType(typeof(BaseCard))]
    public class TestCardCmdletCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Json { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string PsTeamsPath { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string CardType { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 3,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string ChannelsFile { get; set; }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            InitialSessionState initial = InitialSessionState.CreateDefault();
            initial.ImportPSModule(new string[] { PsTeamsPath });
            Runspace runspace = RunspaceFactory.CreateRunspace(initial);
            runspace.Open();
            PowerShell ps = PowerShell.Create();
            ps.Runspace = runspace;
            //ps.AddScript(PsTeamsPath.Replace(".psd", ".psm1"));
            ICard card = null;
            Json = File.ReadAllText(Json);
            switch (CardType)
            {
                case "AdaptiveCard":
                    card = JsonConvert.DeserializeObject<AdaptiveCard>(Json);
                    break;
                case "ListCard":
                    card = JsonConvert.DeserializeObject<ListCard>(Json);
                    break;
                case "ThumbnailCard":
                    card = JsonConvert.DeserializeObject<ThumbnailCard>(Json);
                    break;
                case "HeroCard":
                    card = JsonConvert.DeserializeObject<HeroCard>(Json);
                    break;
            }
            try
            {
                card.SendCard(ref ps, ChannelsFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                ps.Dispose();
                runspace.Close();
                runspace.Dispose();
            }

            //var card = new AdaptiveCard
            //{
            //    Json = Json
            //};
            //WriteObject(card.Json);
            //WriteObject(card);
            //WriteObject(PsTeamsPath);
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class Image
    {
        public string Url { get; set; }
        public string AltUrl { get; set; }
    }

    public interface ICard
    {
        string Json { get; set; }
        string Type { get; set; }
        void SendCard(ref PowerShell ps, string channelsFile);
    }

    public abstract class BaseCard : ICard
    {
        public string Json { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Text { get; set; }
        public Image Image { get; set; }
        public string Uri { get; set; }
        public abstract void SendCard(ref PowerShell ps, string channelsFile);

    }
    public class GenericCard : BaseCard
    {
        public override void SendCard(ref PowerShell ps, string channelsFile)
        {
            throw new NotImplementedException();
        }
    }
    public class AdaptiveCard : BaseCard
    {
        public override void SendCard(ref PowerShell ps, string channelsFile)
        {
            throw new NotImplementedException();
        }
    }
    public class ListCard : BaseCard
    {
        public override void SendCard(ref PowerShell ps, string channelsFile)
        {
            throw new NotImplementedException();
        }
    }
    public class HeroCard : BaseCard
    {
        public override void SendCard(ref PowerShell ps, string channelsFile)
        {
                var lines = System.IO.File.ReadAllLines(channelsFile);
                foreach (var line in lines)
                {
                    var columns = line.Split(';');
                    ScriptBlock scriptBlock = ScriptBlock.Create("{New-HeroImage -Url 'https://upload.wikimedia.org/wikipedia/en/a/a6/Bender_Rodriguez.png' -AltText \"Bender Rodríguez\"}");
                    ps.AddCommand("New-HeroCard")
                    .AddParameter("Title", Title)
                    .AddParameter("SubTitle", SubTitle)
                    .AddParameter("Text", Text)
                    .AddParameter("Content", scriptBlock)
                    .AddParameter("Uri", columns[0]);

                    if (columns[1].Trim() != String.Empty)
                    {
                        ps.Invoke();
                    }
                    
                }
                ps.Dispose();
            }
        }
    public class ThumbnailCard : BaseCard
    {
        public override void SendCard(ref PowerShell ps, string channelsFile)
        {

            #region dead code 
            //var command = "New-ThumbnailCard -Title " + Title + " " +
            //    "-SubTitle \"" + SubTitle + "\" " +
            //    "-Text \"" + Text + "\"" +
            //    "{\r\n    New-ThumbnailImage -Url 'https://upload.wikimedia.org/wikipedia/en/a/a6/Bender_Rodriguez.png' " +
            //    "-AltText \"Bender Rodríguez\"\r\n} -Uri " + Uri + "";
            //ps.AddCommand(command);
            #endregion

            var lines = System.IO.File.ReadAllLines(channelsFile);
            foreach (var line in lines)
            {
                var columns = line.Split(';');
                this.Uri = columns[0];
                ScriptBlock scriptBlock = ScriptBlock.Create("{New-ThumbnailImage -Url 'https://upload.wikimedia.org/wikipedia/en/a/a6/Bender_Rodriguez.png' -AltText \"Bender Rodríguez\"}");
                ps.AddCommand("New-ThumbnailCard")
                .AddParameter("Title", Title)
                .AddParameter("SubTitle", SubTitle)
                .AddParameter("Text", Text)
                .AddParameter("Content", scriptBlock)
                .AddParameter("Uri", this.Uri);

                ps.Invoke();
            }
            ps.Dispose();
        }
    }
}
