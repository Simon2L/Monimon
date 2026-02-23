namespace Monimon.Logic

open System.Diagnostics
open System

module Hardware =
    let getTemp zone : Nullable<double> =
        let path = $"/sys/class/thermal/thermal_zone{zone}/temp"
        if System.IO.File.Exists(path) then
            let raw = System.IO.File.ReadAllText(path) |> float
            Nullable(raw / 1000.0)
        else 
            Nullable()

module Services =
    let control action serviceName =
        let psi = ProcessStartInfo("sudo", $"systemctl {action} {serviceName}")
        psi.RedirectStandardOutput <- true
        psi.UseShellExecute <- false
        let p = Process.Start(psi)
        p.WaitForExit()
        p.ExitCode = 0

type ProjectStatus = 
    | Active of uptime: string
    | Inactive of inactive: string
    | Failed of error: string

module Projects =
    let getStatus (name: string) =
        // Logic to check systemctl status
        // For now, a mock-up:
        if name = "Cloudflare" then Active "2 days"
        else Inactive "inactive"

module ServiceFactory =
    
    let createController serviceName =
        // We explicitly cast to a System.Func so C# recognizes it
        Func<string, bool>(fun action -> 
            Services.control action serviceName
        )