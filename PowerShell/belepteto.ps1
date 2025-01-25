$belepteto = Get-Content .\belepteto.dat
$user = Get-Content .\user.dat

if ($args[0] -eq "-lista"){
    Write-Host A rendszerben szereplő szobák:
    for ($i = 0; $i -lt $belepteto.Length; $i++){
        Write-Host -ForegroundColor Magenta (Write-Output (($belepteto[$i] -split ',')[0]))
    }
}
elseif ($args[0] -eq "-felismer"){
    $user_face = (Select-String -Path "user.dat" -Pattern $args[1]).Line
    if ($user_face.Length -gt 0){
        Write-Output ("Név: " + ($user_face -split ';')[1])
        Write-Output ("Kutató csoport: " + ($user_face -split ';')[2])
        Write-Output ("Jogosultság: " + ($user_face -split ';')[3])
        Write-Output ("Arcképek: " + ($user_face -split ';')[0])
    }
    else{
        Write-Host -ForegroundColor Red A rendszerben nem található a megadott arckép!
    }
}
elseif ($args[0] -eq "-bemehet"){
    $user_face = (Select-String -Path "user.dat" -Pattern $args[1]).Line
    if ($user_face.Length -gt 0){
        $user_access = ($user_face -split ';')[3]
    }
    $room_access = (Select-String -Path "belepteto.dat" -Pattern $args[2]).Line
    if ($room_access.Length -gt 0){
        $room_access = ($room_access.Replace(" ", "") -split ',')[1]
    }
    
    switch ($room_access){
    "A"{
        if ($user_access -eq $room_access){
            Write-Host -ForegroundColor Green Bemehet
        }
        elseif ($user_face.Length -eq 0){
            Write-Host -ForegroundColor Red Nincs ilyen arckép!
        }
        else{
            Write-Host -ForegroundColor Red Nem mehet be
        }
    }

    "B"{
        if (($user_access -eq $room_access) -or ($user_access -eq "A")){
            Write-Host -ForegroundColor Green Bemehet
        }
        elseif ($user_access.Length -eq 0){
           Write-Host -ForegroundColor Red Nincs ilyen arckép!
        }
        else{
            Write-Host -ForegroundColor Red Nem mehet be
        }
    }

    "C"{
        if (($user_access -eq $room_access) -or ($user_access -eq "A") -or ($user_access -eq "B")){
            Write-Host -ForegroundColor Green Bemehet
        }
        elseif ($user_access.Length -eq 0){
            Write-Host -ForegroundColor Red Nincs ilyen arckép!
        }
        else{
            Write-Host -ForegroundColor Red Nem mehet be
        }
    }

    default{
        if ($user_access.Length -eq 0){
            Write-Host -ForegroundColor Red Nincs ilyen szoba és arckép!
        }
        else{
            Write-Host -ForegroundColor Red Nincs ilyen szoba!
        }
    }
    }
}
else{
    Write-Host -ForegroundColor Red Nincs ilyen kapcsoló
}