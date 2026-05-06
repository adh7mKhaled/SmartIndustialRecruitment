# دليل مشروع Smart Industrial Recruitment (Backend)

هذا الملف يحتوي على شرح مفصل للمشروع، الأدوار المتاحة، والـ Endpoints الخاصة بالنظام.

## 1. نبذة عن المشروع
مشروع **Smart Industrial Recruitment** هو منصة تهدف لتسهيل عملية التوظيف في القطاع الصناعي، حيث يربط بين أصحاب العمل (Employers) والعمال (Workers). يعتمد المشروع على تقنيات **.NET Core Web API** مع استخدام **Entity Framework Core** لإدارة قاعدة البيانات و **ASP.NET Core Identity** لتأمين النظام باستخدام **JWT Tokens**.

---

## 2. الأدوار في النظام (Roles)
يحتوي النظام على ثلاثة أدوار رئيسية:
1.  **Admin (المسؤول):** له صلاحيات كاملة لإدارة النظام (سيتم تفعيلها لاحقاً بشكل أكبر).
2.  **Employer (صاحب العمل):** يمكنه إضافة وظائف، تعديلها، حذفها، ومراجعة طلبات التوظيف المقدمة على وظائفه.
3.  **Worker (العامل):** يمكنه إنشاء ملف شخصي، إضافة مهاراته، تصفح الوظائف المتاحة، والتقديم عليها.

---

## 3. شرح الـ Endpoints

### أ. المصادقة والتوثيق (Authentication)
تتم عبر `AuthController`:
- `POST /Auth/login`: تسجيل الدخول (عن طريق الإيميل أو رقم الهاتف).
- `POST /Auth/register/worker`: تسجيل حساب جديد لعامل.
- `POST /Auth/register/employer`: تسجيل حساب جديد لصاحب عمل.
- `POST /Auth/refresh-token`: تجديد الـ Access Token باستخدام Refresh Token.
- `POST /Auth/confirm-email`: تأكيد البريد الإلكتروني.
- `POST /Auth/forget-password` & `POST /Auth/reset-password`: استعادة كلمة المرور.

### ب. الوظائف (Jobs)
تتم عبر `JobsController`:
- `GET /api/Jobs`: عرض كل الوظائف المتاحة (متاح للجميع مع إمكانية الفلترة بالمدينة أو التصنيف).
- `GET /api/Jobs/{id}`: عرض تفاصيل وظيفة معينة.
- `GET /api/Jobs/my-jobs`: (لصاحب العمل) عرض الوظائف التي قام بنشرها.
- `POST /api/Jobs`: (لصاحب العمل) إضافة وظيفة جديدة.
- `PUT /api/Jobs/{id}`: (لصاحب العمل) تعديل بيانات وظيفة.
- `DELETE /api/Jobs/{id}`: (لصاحب العمل) حذف وظيفة.

### ج. طلبات التوظيف (Job Applications)
تتم عبر `JobApplicationsController`:
- `POST /api/JobApplications/apply`: (للعامل) التقديم على وظيفة معينة.
- `GET /api/JobApplications/my-applications`: (للعامل) عرض الطلبات التي قدمها وحالتها.
- `GET /api/JobApplications/job/{jobId}`: (لصاحب العمل) عرض كل المتقدمين لوظيفة معينة.
- `PATCH /api/JobApplications/{applicationId}/status`: (لصاحب العمل) تغيير حالة الطلب (مقبول، مرفوض، قيد المراجعة، إلخ).

### د. مهارات العمال (Worker Skills)
تتم عبر `WorkerSkillsController`:
- `GET /api/WorkerSkills`: عرض مهارات العامل الحالي.
- `POST /api/WorkerSkills`: إضافة مهارة جديدة للعامل.
- `PUT /api/WorkerSkills/{id}`: تعديل بيانات مهارة موجودة.
- `DELETE /api/WorkerSkills/{id}`: حذف مهارة.

### هـ. التصنيفات (Categories)
تتم عبر `CategoriesController`:
- `GET /api/Categories`: عرض كل تصنيفات الوظائف المتاحة (مثل: حدادة، نجارة، لحام، إلخ).

---

## 4. تفاصيل تقنية إضافية
- **BaseService & Generic Results:** يتم استخدام نمط الـ Result Pattern لتوحيد شكل الردود (Responses) والتعامل مع الأخطاء بشكل احترافي.
- **Fluent Validation:** يتم التأكد من صحة البيانات المرسلة في كل Request.
- **Soft Delete:** بعض الكيانات تدعم الحذف الناعم (IsDeleted) للحفاظ على سلامة البيانات.
- **Audit Fields:** يتم تسجيل وقت الإنشاء والتعديل لكل سجل في قاعدة البيانات تلقائياً.
