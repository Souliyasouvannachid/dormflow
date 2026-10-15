const fs=require('fs'),path=require('path');
const specs=[
['organizations','องค์กร','Organizations','CreateOrganizationRequest','UpdateOrganizationRequest','name,code,status','องค์กร'],
['dormitories','หอพัก','Dormitories','CreateDormitoryRequest','UpdateDormitoryRequest','name,code,phone,waterRatePerUnit,electricityRatePerUnit','ห้องพัก'],
['buildings','อาคาร','Buildings','CreateBuildingRequest','UpdateBuildingRequest','name,code,totalFloors,status','ห้องพัก'],
['floors','ชั้น','Floors','CreateFloorRequest','UpdateFloorRequest','name,floorNumber,status','ห้องพัก'],
['room-types','ประเภทห้อง','RoomTypes','CreateRoomTypeRequest','UpdateRoomTypeRequest','name,monthlyRent,depositAmount,maximumOccupants','ห้องพัก'],
['rooms','Room Center','Rooms','CreateRoomRequest','UpdateRoomRequest','roomNumber,buildingName,floorName,roomTypeName,monthlyRent,status','ห้องพัก'],
['tenants','ผู้เช่า','Tenants','CreateTenantRequest','UpdateTenantRequest','fullName,phone,email,emergencyContactName,status','การเข้าพัก'],
['reservations','จองห้อง','Reservations','CreateReservationRequest',null,'roomId,tenantId,moveInDate,expiresAt,status','การเข้าพัก'],
['contracts','สัญญาเช่า','Leases','CreateContractRequest',null,'contractNumber,roomId,tenantId,startDate,endDate,monthlyRent,status','การเข้าพัก'],
['meter-readings','จดมิเตอร์','MeterReadings','CreateMeterReadingRequest',null,'roomId,meterType,periodMonth,previousReading,currentReading,unitsUsed','การเงิน'],
['billing-periods','รอบบิล','Operations',null,null,'month,dormitoryId,isClosed','การเงิน'],
['invoices','ใบแจ้งหนี้','Invoices','CreateInvoiceRequest',null,'invoiceNumber,periodMonth,totalAmount,paidAmount,remainingBalance,status','การเงิน'],
['payments','รับชำระเงิน','Payments','CreatePaymentRequest',null,'invoiceId,amount,method,paymentDate,status','การเงิน'],
['receipts','ใบเสร็จ','Receipts',null,null,'receiptNumber,amount,issuedAt,paymentId','การเงิน'],
['expenses','ค่าใช้จ่าย','Expenses','CreateExpenseRequest',null,'expenseDate,category,description,amount,paidTo','การเงิน'],
['maintenance-requests','งานซ่อม','Maintenance','CreateMaintenanceRequestRequest',null,'title,roomId,category,priority,status,actualCost','งานประจำวัน'],
['parcels','พัสดุ','Parcels','LogParcelRequest',null,'trackingNumber,courierName,roomId,status,receivedAt','งานประจำวัน'],
['announcements','ประกาศ','Announcements','CreateAnnouncementRequest',null,'title,priority,status,publishedAt','งานประจำวัน'],
['notifications','การแจ้งเตือน','Notifications',null,null,'title,message,isRead,createdAt','งานประจำวัน'],
['users','ผู้ใช้งาน','Users','CreateUserRequest','UpdateUserRequest','fullName,email,status,roles','ตั้งค่า'],
['roles','บทบาทและสิทธิ์','Roles','CreateRoleRequest','UpdateRoleRequest','name,description','ตั้งค่า'],
['settings','ตั้งค่าระบบ','Settings',null,null,'key,value','ตั้งค่า'],
];
function fields(folder,file){if(!file)return[];const p=`src/DormFlow.Contracts/${folder}/${file}.cs`;if(!fs.existsSync(p))throw Error(p);const txt=fs.readFileSync(p,'utf8');return[...txt.matchAll(/public\s+(\w+(?:<[^>]+>)?(?:\[\])?\??)\s+(\w+)\s*\{\s*get;/g)].map(m=>({key:m[2][0].toLowerCase()+m[2].slice(1),type:m[1],optional:m[1].includes('?'),array:/\[\]|List|Collection/.test(m[1])})).filter(f=>!f.key.startsWith('floorPlan')&&f.key!=='actualCost');}
const resources=specs.map(([key,title,folder,create,update,columns,group])=>({key,title,group,columns:columns.split(','),fields:fields(folder,create),updateFields:fields(folder,update),canCreate:!!create,canUpdate:!!update}));
fs.mkdirSync('web/src',{recursive:true});fs.writeFileSync('web/src/schema.json',JSON.stringify(resources,null,2));
