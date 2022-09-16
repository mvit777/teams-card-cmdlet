# teams-card-cmdlet

A sample c# library to wrap the excellent [PSTeams](https://github.com/EvotecIT/PSTeams) module

### what is PSTeams, by the way?

In their very own definition

***"PSTeams is a PowerShell Module working on Windows / Linux and Mac. It allows sending notifications to Microsoft Teams via WebHook Notifications"***

It actually supports 4 types of cards:
- Adaptive Card
- List Card
- Hero Card
- Thumbnail Card

The aim of my library is to simplify the sending of a card from

```powershell
New-ThumbnailCard -Title 'Bender' -SubTitle "tale of a robot who dared to love" -Text "Bender Bending Rodríguez is a main character in the animated television series Futurama. He was created by series creators Matt Groening and David X. Cohen, and is voiced by John DiMaggio" {
    New-ThumbnailImage -Url 'https://upload.wikimedia.org/wikipedia/en/a/a6/Bender_Rodriguez.png' -AltText "Bender Rodríguez"
    New-ThumbnailButton -Type imBack -Title 'Thumbs Up' -Value 'I like it' -Image "http://moopz.com/assets_c/2012/06/emoji-thumbs-up-150-thumb-autox125-140616.jpg"
    New-ThumbnailButton -Type openUrl -Title 'Thumbs Down' -Value 'https://evotec.xyz'
    New-ThumbnailButton -Type openUrl -Title 'I feel luck' -Value 'https://www.bing.com/images/search?q=bender&qpvt=bender&qpvt=bender&qpvt=bender&FORM=IGRE'
} -Uri $Env:TEAMSPESTERID
```
to
```powershell
.\main.ps1
```
and then a bunch of mixed interactive questions on the shell and some gui for channel manager and an optional blazor gui for non-techie users.
Content and structure, as said, can also be edited on the fly directly in vscode as a .yml file. That happens during the run of the main.ps1 script.

## How to use the library
Examples of how to use the library from powershell, blazor or any other client can be seen in the [PSCardRage repository](https://github.com/mvit777/psroids)

## Brief explanation of the code
Turns out that in powershell is possible to use a .NET custom library and from that library it is also possible to invoke ps scripts or ps modules.

To include the .NET lib in your ps script it is only necessary to add this line at top of powershell script
```powershell
$dllRoot = 'C:\<PATH_TO>\PowerShellModuleTest\PowerShellModuleTest.Cmdlet\bin\Debug\netstandard2.0'
Import-Module -Name $dllRoot\PowerShellModuleTest.Cmdlet.dll
```
and you can use it straight away as powershell module (.psm1)
As I learnt from [this execellent article](https://www.terrybutler.co.uk/2021/08/12/creating-powershell-module-csharp/) it is then just necessary to import 
a couple of namespaces and create a Cmdlet class with properties as (optional) parameters
```csharp
(...other namespaces omitted for brevity..)
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PowerShellModuleTest.Cmdlet{
[Cmdlet(VerbsDiagnostic.Test, "CardCmdlet")]
    [OutputType(typeof(BaseCard))]
    public class TestCardCmdletCommand : PSCmdlet

```
with a very precise structure
```csharp

```
