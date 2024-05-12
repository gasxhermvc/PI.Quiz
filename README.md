# PI.Quiz
Project ����Ѻ���ͺ����ѷ PI Securities By Thanatmet thanarattanan

## System Architecture
1. N-Tier Application

## System Development
1. Best Practice API Designer
2. Code First

## Technology Stack
1. NET CORE 8.0
2. SQLite Database
3. NLog
4. Postman

## Strcuture Details
1. PI.Quiz.Presentation �繪�� Layer ���ش����Ѻ�繪�鹵Դ��ͻ���ҹ�Ѻ Client
2. PI.Quiz.Business �繪�� Layer �ͧŧ�ҡ Presentation ��˹�ҷ���� Business logic �ͧ�к�
3. PI.Quiz.DAL �繪�� Layer �ӴѺ��� 3 ����Ѻ��˹�ҷ������ǡѺ �ҹ������ ��� Data �ͧ�к�
4. PI.Quiz.UnitTest �� Layer �ͧ�ش���·ӧҹ�����Ѻ Engine ����Ѻ Script ���ͺ
5. PI.Quiz.Engine �繪�� Layer �ش���·�˹�ҷ���� Core �Ӥѭ㹡������ԡ�áѺ��� 4 Layers ����٧����

## How to run program for development
1. ��������� Add-Migration � Project PI.Quiz.DAL
2. ��������� Update-Database � Project PI.Quiz.DAL
3. Run program ���� with debugging (F5) ���� without debugging (Ctrl+F5)
4. Connect �ҹ�����Ŵ��� SQLiteStudio 3.4.4 ��ѧ��� app.db
5. Import postmand collection ��� Environment ���ͷ��ͺ�ԧ API
6. ���ͺ�ԧ API token �������ҧ Token ��͹�ҡ��鹷��ͺ��ҹ UserManagement ��ǹ�ͧ��ô֧ List ��¡��
#### �����˵�: 
1. ������ա�� Build ������� Program �ФѴ�͡��� app.db ��ҧ�ѧ Path run application �ѵ��ѵ�
2. ������� Development mode ��������� ������ҧ��� app.db ����ҡ������� ��е�ͧ����� Migration �����ա���� 

### Install migration
- ����Ѻ ��õԴ��� Migrations ���Ѻ Application �¢�鹵͹���е�ͧ����� app.db ��дѺ Root Application �����ҡ����������礵����������ա�� Migrations ���º��������������� �����������Դ Error ���͢����ū�� ����ö Remove-migration ��͹˹���ͺ��
Add-Migration PI_QUIZ

### Create seed data
- ����Ѻ�óյ�ͧ��� Mockup data ���������ͨҡ�������ö������ҹ��� Seed Db �� �ҡ�������ѹ����� Update-Database
Add-Migration SeedInitialData

### Update database
- ����Ѻ�óյ�ͧ����ѻവ�ҹ�����������Ҩ��繡�� Add-Migration ���� Seed Db ����ö�ѹ�ѻവ������
Update-Database

### Remove migration
- ����Ѻ�͹��õԴ��� Table ��ҧ � �������ա���ѹ Update-Database ���͵Դ��駵��ҧ�ҹ�����������ա����
Remove-Migration -f

## Unit Test & E2E Test
- ��÷��ͺ Unit Test ����ҹ NUnit.Framework
- ��÷��ͺẺ E2E ���� Postman Run Collection

## Flow
### CSRF
- �ԧ���� Generate CSRF ����Ѻ��㹡���� Form ���� Request ���� Security ���Ѻ Website ���Դ��ҹ�繵�����ҧ� Acccount -> Register ��ҹ��

### Authen
- Login: �ԧ API PI.Quiz -> Authen -> token �Ѻ Response ��������ö��ҹ���������ա�� Setup AccessToken ��� RefreshToken Ẻ Auto ��ҹ Postman tools
- Refresh token: �ԧ API PI.Quiz -> Authen -> token ������ԧ API PI.Quiz -> Authen -> refresh-token �� Key �����͹�á����ҹ����§����������ѧ�ҡ Refresh token ���� Key �ж١�����
- UserInfo: �� AccessToken �����ҡ Login ���ԧ API PI.Quiz -> Authen -> user-info 
- Revoke: �� AccessToken �����ҡ Login ���ԧ API PI.Quiz -> Authen -> revoke ����ѧ�ҡ�ԧ Access Token �ж١������������ö��ҹ���ա 

### Account
- Register: �ԧ API PI.Quiz -> CSRF -> generator ���价�� API PI.Quiz -> Account -> register ��͡�����������ԧ�������ҧ User
- Forgot password: ��ѧ�ҡ��Ѥ� User �������� Email ������� Password ���� Token ���ͧ (���� Token �е�ͧ��价ҧ Email �����ͧ�ҡ����� MailServer �����ҹ�֧��������ǹ���)
- Reset password: �� Token �����ҡ�ԧ API PI.Quiz -> Account -> forgot-password ���ҧ��� URL ��С�˹� newPassword 

### UM
- Lists: ��͡�Թ���� User ����� Role �� SuperAdmin ���� Admin ��ҹ�����ʹ֧ Users ���Ҷ��˹� keyword ���ͤ��� ¡������ҧ��˹��� Thanatmet ���� thanatmet �繵� ���к�����ҷ�����, ���ʡ�� ��Ъ������ ��ҹ��
- Update: �к����ѻവ User ���ç�Ѻ��к�
- Delete: �к��зӡ�� Set Delete Flag ���Ѻ��¡�÷��������ź


## Deployment
���ǹ�ͧ��� Deployment Source Code 价������ͧ Server ��ҧ� ����ö���͡�����㹡�� Deploy �ҡ��ҹ��ҧ������� Source Code �������� Directory: pea3.webservice/bin/Release/net6.0/Publish ���ѡ�Ѳ�� Copy ������������ Folder Publish ��ҧ�� Server �¡�͹�ҧ������ӡ�� Stop App Pool "pea3.webservice" ��͹�ء���� ����ͷѺ�������ӡ�� Start ��Ѻ��
```cmd
P1 > dotnet publish
P2 > dotnet publish --configuration Release /p:EnvironmentName=Development
P3 > dotnet publish --configuration Release /p:EnvironmentName=Inhouse
P4 > dotnet publish --configuration Release /p:EnvironmentName=Uat
P5 > dotnet publish --configuration Release /p:EnvironmentName=Production
```
#### �����˵�: 
���͡�ٻẺ P1-P5 ���ҧ����ҧ˹�� �¡���ѹẺ P1 ���� Environment Default �� Production ��� P2 �繡�� Deploy �� IIS �ͧ����ͧ�ѡ�Ѳ���ͧ

## Case Cannot Listening on Port 7012
* �ӡ�� Activate Certification �ա�ͺ���¤������ҧ
* $ dotnet dev-certs https --trust

## Author Infomation
* Developer: Thanatmet Thanarattanan
* Code Name: GasxherMvc
* Email: dev.awesome.th@gmail.com