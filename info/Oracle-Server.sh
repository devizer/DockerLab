
# RUN CONTAINRT
docker run -it --name ora -p 1521:1521 store/oracle/database-enterprise:12.2.0.1

DB/SID: ORCLCDB
User: SYS
Password: Oradoc_db1

# LOGIN
docker exec -it ora bash

log file is : /home/oracle/setup/log/paramChk.log
paramChk.sh is done at 0 sec

untar DB bits ......
log file is : /home/oracle/setup/log/untarDB.log



yum -y install ncurses-devel
wget http://hisham.hm/htop/releases/2.0.1/htop-2.0.1.tar.gz
tar -xvf htop-2.0.1.tar.gz
cd htop-2.0.1
./configure
make && make install
