# PI.Quiz
Project สำหรับทดสอบบริษัท PI Securities By Thanatmet thanarattanan

## System Architecture
1. N-Tier Application

## System Development
1. Best Practice API Designer
2. Code First

## Technology Stack
1. NET CORE 8.0
2. SQLite Database
3. NLog
4. NUnit
5. Postman

## Strcuture Details
1. PI.Quiz.Presentation เป็นชั้น Layer บนสุดสำหรับเป็นชั้นติดต่อประสานกับ Client
2. PI.Quiz.Business เป็นชั้น Layer รองลงจาก Presentation ทำหน้าที่เก็บ Business logic ของระบบ
3. PI.Quiz.DAL เป็นชั้น Layer ลำดับที่ 3 สำหรับทำหน้าที่เกี่ยวกับ ฐานข้อมูล และ Data ของระบบ
4. PI.Quiz.UnitTest เป็น Layer รองสุดท้ายทำงานร่วมกับ Engine สำหรับ Script ทดสอบ
5. PI.Quiz.Engine เป็นชั้น Layer สุดท้ายทำหน้าที่เป็น Core สำคัญในการให้บริการกับทั้ง 4 Layers ที่สูงขึ้นไป

## How to run program for development
1. พิมพ์คำสั่ง Add-Migration ณ Project PI.Quiz.DAL
2. พิมพ์คำสั่ง Update-Database ณ Project PI.Quiz.DAL
3. Run program ด้วย with debugging (F5) หรือ without debugging (Ctrl+F5)
4. Connect ฐานข้อมูลด้วย SQLiteStudio 3.4.4 ไปยังไฟล์ app.db
5. Import postmand collection และ Environment เพื่อทดสอบยิง API
6. ทดสอบยิง API token เพื่อสร้าง Token ก่อนจากนั้นทดสอบใช้งาน UserManagement ส่วนของการดึง List รายการ
#### หมายเหตุ: 
1. เมื่อมีการ Build เสร็จสิ้น Program จะคัดลอกไฟล์ app.db ไปวางยัง Path run application อัตโนมัติ
2. เมื่อเป็น Development mode โปรแกรมจะเช็ค และสร้างไฟล์ app.db ให้หากไฟล์หายไป แต่จะต้องเริ่ม Migration ใหม่อีกครั้ง 

### Install migration
- สำหรับ การติดตั้ง Migrations ให้กับ Application โดยขั้นตอนนี้จะต้องเช็คไฟล์ app.db ในระดับ Root Application ด้วยหากมีแล้วให้เช็คต่อว่าในไฟล์มีการ Migrations เรียบร้อยแล้วหรือไม่ เพื่อไม่ให้เกิด Error หรือข้อมูลซ้ำ สามารถ Remove-migration ก่อนหนึ่งรอบได้
```cmd 
Add-Migration PI_QUIZ
`

### Create seed data
- สำหรับกรณีต้องการ Mockup data เพิ่มเติมต่อจากเดิมสามารถเพิ่มผ่านการ Seed Db ได้ จากนั้นให้รันคำสั่ง Update-Database
```cmd 
Add-Migration SeedInitialData
`

### Update database
- สำหรับกรณีต้องการอัปเดตฐานข้อมูลไม่ว่าจะเป็นการ Add-Migration หรือ Seed Db สามารถรันอัปเดตได้เสมอ
```cmd 
Update-Database
`

### Remove migration
- สำหรับถอนการติดตั้ง Table ต่าง ๆ ที่เรามีการรัน Update-Database เพื่อติดตั้งตารางฐานข้อมูลใหม่อีกครั้ง
```cmd 
Remove-Migration -f 
`

## Unit Test & E2E Test
- การทดสอบ Unit Test จะใช้งาน NUnit.Framework
- การทดสอบแบบ E2E จะใช้ Postman Run Collection

## Flow
### CSRF
- ยิงเพื่อ Generate CSRF สำหรับใช้ในการส่ง Form หรือ Request เพิ่ม Security ให้กับ Website มีเปิดใช้งานเป็นตัวอย่างใน Acccount -> Register เท่านั้น

### Authen
- Login: ยิง API PI.Quiz -> Authen -> token รับ Response แล้วสามารถใช้งานต่อได้เลยมีการ Setup AccessToken และ RefreshToken แบบ Auto ผ่าน Postman tools
- Refresh token: ยิง API PI.Quiz -> Authen -> token และมายิง API PI.Quiz -> Authen -> refresh-token โดย Key ที่ได้ตอนแรกจะใช้งานได้เพียงครั้งเดียวหลังจาก Refresh token แล้ว Key จะถูกทำราย
- UserInfo: นำ AccessToken ที่ได้จาก Login มายิง API PI.Quiz -> Authen -> user-info 
- Revoke: นำ AccessToken ที่ได้จาก Login มายิง API PI.Quiz -> Authen -> revoke โดยหลังจากยิง Access Token จะถูกทำลายไม่สามารถใช้งานได้อีก 

### Account
- Register: ยิง API PI.Quiz -> CSRF -> generator และไปที่ API PI.Quiz -> Account -> register กรอกข้อมูลแล้วยิงเพื่อสร้าง User
- Forgot password: หลังจากสมัคร User แล้วให้นำ Email มาแจ้งลืม Password จะได้ Token จำลอง (ปกติ Token จะต้องส่งไปทาง Email แต่เนื่องจากไม่มี MailServer ให้ใช้งานจึงไม่ได้ทำส่วนนี้)
- Reset password: นำ Token ที่ได้จากยิง API PI.Quiz -> Account -> forgot-password มาวางที่ URL และกำหนด newPassword 

### UM
- Lists: ล็อกอินด้วย User ที่มี Role เป็น SuperAdmin หรือ Admin เท่านั้นเพื่อดึง Users สามาถกำหนด keyword เพื่อค้นหา ยกตัวอย่างกำหนดเป็น Thanatmet หรือ thanatmet เป็นต้น โดยระบบจะไปหาที่ชื่อ, นามสกุล และชื่อเล่น เท่านั้น
- Update: ระบบจะอัปเดต User ที่ตรงกับในระบบ
- Delete: ระบบจะทำการ Set Delete Flag ให้กับรายการที่ส่งเข้าไปลบ


## Deployment
ในส่วนของการ Deployment Source Code ไปที่เครื่อง Server ต่างๆ สามารถเลือกคำสั่งในการ Deploy จากด้านล่างได้เลยโดย Source Code จะอยู่ที่ Directory: pea3.webservice/bin/Release/net6.0/Publish ให้นักพัฒนา Copy ไฟล์ทั้งหมดภายใน Folder Publish ไปวางบน Server โดยก่อนวางไฟล์ให้ทำการ Stop App Pool "pea3.webservice" ก่อนทุกครั้ง เมื่อทับเสร็จให้ทำการ Start กลับมา
```cmd
P1 > dotnet publish
P2 > dotnet publish --configuration Release /p:EnvironmentName=Development
P3 > dotnet publish --configuration Release /p:EnvironmentName=Inhouse
P4 > dotnet publish --configuration Release /p:EnvironmentName=Uat
P5 > dotnet publish --configuration Release /p:EnvironmentName=Production
```
#### หมายเหตุ: 
เลือกรูปแบบ P1-P5 อย่างใดอย่างหนึ่ง โดยการรันแบบ P1 จะได้ Environment Default เป็น Production

## Case Cannot Listening on Port 7012
* ทำการ Activate Certification อีกรอบด้วยคำสั่งล่าง
* $ dotnet dev-certs https --trust

## Author Infomation
* Developer: Thanatmet Thanarattanan
* Code Name: GasxherMvc
* Email: dev.awesome.th@gmail.com