$file = "C:\git\VolpeCCReact\bin\Debug\VolpeCCReact.cpz"
$ip = "192.168.0.159"
$slotNumber = 1
$username = "admin"
$password = "Tritech1!"

Send-CrestronProgram -Device $ip -LocalFile $file -Password $password -ProgramSlot $slotNumber -Secure -Username $username -ShowProgress 
