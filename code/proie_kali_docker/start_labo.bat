@echo off
chcp 65001 >nul 2>&1
cd /d "%~dp0"

echo ======================================================================
echo   Laboratoire 2.9 - Demarrage (Proie + Kali)
echo ======================================================================

REM --- Verifier que Docker Desktop est demarre ---
docker info >nul 2>&1
if errorlevel 1 (
  echo.
  echo ERREUR: Docker Desktop ne repond pas.
  echo Lancez Docker Desktop, attendez qu'il soit pret, puis relancez ce script.
  echo.
  pause
  exit /b 1
)

REM --- Nettoyage d'anciens conteneurs ---
docker rm -f ctf-proie ctf-kali-gui >nul 2>&1

REM --- Reseau 192.168.220.0/24 si absent ---
docker network inspect labo_net >nul 2>&1
if errorlevel 1 (
  echo Creation du reseau labo_net 192.168.220.0/24 ...
  docker network create --subnet=192.168.220.0/24 labo_net
)

echo.
echo Construction de la machine Proie ...
docker build -t ctf-proie ".\proie"
if errorlevel 1 goto erreur

echo.
echo Construction de la machine Kali GUI ...
docker build -t ctf-kali-gui ".\kali"
if errorlevel 1 goto erreur

echo.
echo Demarrage de la Proie ...
docker run -d --name ctf-proie --hostname proie-ssh --cap-add=NET_ADMIN --net labo_net --ip 192.168.220.128 -p 3001:3000 -p 2222:22 ctf-proie
if errorlevel 1 goto erreur

echo Demarrage de Kali Linux ...
docker run -d --name ctf-kali-gui --hostname kali-predateur --privileged --net labo_net --ip 192.168.220.129 -p 3000:3000 --shm-size=1gb ctf-kali-gui
if errorlevel 1 goto erreur

echo.
echo ======================================================================
echo   Laboratoire demarre avec succes !
echo.
echo   Interface Kali  : http://localhost:3000
echo   Interface Proie : http://localhost:3001   (ecran de connexion)
echo.
echo   Cible SSH (depuis le terminal Kali) : 192.168.220.128  port 22
echo   Cible SSH (depuis Windows)          : localhost        port 2222
echo.
echo   Terminal admin de la Proie : docker exec -it ctf-proie bash
echo ======================================================================
echo.
pause
exit /b 0

:erreur
echo.
echo *** Une erreur est survenue pendant la construction ou le demarrage. ***
echo *** Verifiez que Docker Desktop est bien demarre et reessayez.       ***
echo.
pause
exit /b 1
