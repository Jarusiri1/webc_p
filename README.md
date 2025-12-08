#WebACS - ระบบจัดการสิทธิ์ผู้ใช้งานของ PEA 
ระบบพัฒนาโดย ASP.NET Core Razor Pages + SQL Server สำหรับควบคุมสิทธิ์การเข้าถึงแอปพลิเคชันภายในองค์กร โดยมีระบบกลุ่มผู้ใช้และสิทธิ์การใช้งานแบบละเอียด

---

โครงสร้างระบบ
###หน้า Login (`Login.cshtml + .cs`)
- เชื่อม `tblEmployee` เพื่อตรวจสอบ `EmployeeNo`
- ถ้า login สำเร็จ → บันทึก Session: `EmployeeNo`, `FullName`
- หลัง Login → redirect ไป `/App`
```csharp
var emp = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == EmployeeNo);
HttpContext.Session.SetString("EmployeeNo", emp.EmployeeNo);

###แอปพลิเคชัน (App.cshtml + AppModel.cs)
ดึงข้อมูลจาก tblApplication
เมื่อผู้ใช้กดเพิ่มแอป → บันทึกลง tblApplication และบันทึกผู้สร้าง (จาก Session)
เชื่อมกับ ApplicationAdmin (ตาราง: tblApplicationAdmin) เพื่อเก็บว่าใครสร้างแอปนี้
NewApplication.CreateBy = employeeNo;
_context.Applications.Add(NewApplication);

###สิทธิการใช้งาน (Permissions.cshtml + PermissionsModel.cs)
เชื่อม tblPermission ← เก็บสิทธิ์ของแอปแต่ละตัว
ใช้ ApplicationId เพื่อเชื่อมกับแอปพลิเคชัน
มีการเช็ค Session เพื่อให้เห็นเฉพาะสิทธิ์ของพนักงานที่ล็อกอินอยู่
Permissions = _context.Permissions
    .Where(p => p.CreateBy == employeeNo)
    .ToList();

###กลุ่มผู้ใช้งาน (Group.cshtml + GroupModel.cs)
เชื่อม tblGroup ← ใช้เก็บชื่อกลุ่ม, คำอธิบาย
มีการอ้างอิง ApplicationId เพื่อระบุว่าแต่ละกลุ่มใช้กับแอปไหน

###จัดกลุ่มผู้ใช้งาน (UserGroup.cshtml + UserGroupsModel.cs)
เชื่อม tblUserGroup ← ระบุว่า EmployeeNo ใดอยู่ใน GroupId ใด
มี dropdown ให้เลือกชื่อกลุ่มจาก tblGroup
ข้อมูลพนักงานมาจาก tblEmployee
var query = from ug in _context.UserGroups
            join g in _context.Groups on ug.GroupId equals g.GroupId
            join e in _context.Employees on ug.EmployeeNo equals e.EmployeeNo
            select new { ... };
