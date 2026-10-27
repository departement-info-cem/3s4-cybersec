@echo off
chcp 65001 >nul 2>&1
cd /d "%~dp0"

echo ======================================================================
echo   Laboratoire 2.9 - Demarrage (Proie + Kali)
echo ======================================================================

REM --- Verifier que le demon Docker repond (docker ps = 0 si le daemon est joignable).
REM NB: on evite "docker info" qui renvoie parfois un code != 0 a cause de simples
REM warnings, meme quand Docker fonctionne.
docker ps >nul 2>&1
if errorlevel 1 (
  echo.
  echo ERREUR: Docker ne repond pas.
  echo  - Lancez Docker Desktop et attendez qu'il affiche "Running".
  echo  - S'il vient d'etre installe, redemarrez le PC (mise a jour du PATH).
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
echo Demarrage immediat de la Proie ...
docker run -d --name ctf-proie --hostname proie-ssh --cap-add=NET_ADMIN --net labo_net --ip 192.168.220.128 -p 3001:3000 -p 2222:22 ctf-proie
if errorlevel 1 goto erreur

REM --- Attendre que la proie soit prete, puis ouvrir un terminal sur auth.log ---
echo Attente de la proie ...
set /a _try=0
:waitproie
docker exec ctf-proie test -f /var/log/auth.log >nul 2>&1
if not errorlevel 1 goto proieok
set /a _try+=1
if %_try% geq 20 goto proieok
ping -n 2 127.0.0.1 >nul
goto waitproie
:proieok
echo Ouverture du terminal de surveillance (auth.log) ...
start "Proie - auth.log" cmd /k docker exec -it ctf-proie bash -lc "tail -f /var/log/auth.log; exec bash"

echo.
echo Construction de la machine Kali GUI ...
docker build -t ctf-kali-gui ".\kali"
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
