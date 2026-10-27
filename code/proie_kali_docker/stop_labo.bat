@echo off
chcp 65001 >nul 2>&1

echo Arret et suppression des conteneurs du laboratoire ...
docker rm -f ctf-proie ctf-kali-gui >nul 2>&1
docker network rm labo_net >nul 2>&1
echo Environnement nettoye.
echo.
pause
