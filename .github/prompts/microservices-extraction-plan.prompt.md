---
description: צור תוכנית קונקרטית לפיצול המונוליט DotanBooks למיקרוסרביסים לפי הארכיטקטורה הקיימת.
---

# תוכנית חילוץ למיקרוסרביסים - DotanBooks

> תכנון בלבד. אין לממש קוד בפועל.
> הפלט צריך להיות מסמך ברור שמגדיר איך מפצלים את DotanBooks למיקרוסרביסים.

## מטרה

להפיק תוכנית ארכיטקטונית לפיצול המונוליט הקיים של DotanBooks, לפי מבנה הפרויקט בפועל:

- שכבת API: Controllers ו-Middlewares בתוך `DotanBooks/`
- שכבת שירותים: לוגיקה עסקית בתוך `Service/`
- שכבת רפוזיטוריז: גישה לנתונים בתוך `Repository/`
- חוזים: DTOs בתוך `DTOs/`
- מודל דומיין: Entities בתוך `Entitiys/`

## כללי פלט

- הפק מסמך Markdown יחיד.
- שמור על תכנון מעשי, אבל ללא מימוש קוד.
- השתמש בדיוק בסדר הסעיפים שמופיע בהמשך.
- השתמש בשמות אמיתיים מהריפו, לא בדוגמאות כלליות.

## Bounded Contexts מוצעים

השתמש בטבלה הזו כבסיס וחדד אותה רק אם יש הצדקה ברורה.

| שירות | ישויות עיקריות | Controllers נוכחיים | אחריות |
|-------|-----------------|----------------------|--------|
| **IdentityService** | Customer | UsersController | הרשמה, התחברות, JWT, חסימת משתמשים |
| **CatalogService** | Book, Author, Category | BookPageController, SearchBooksController, CategoriesController, GetCategoriesController, AuthorsController, ManagementBookController | קטלוג ספרים, דפדוף, סינון, חיפוש וניהול קטלוג (כולל write-side בשלב הראשון) |
| **OrderService** | Order, OrderItem, OrderStatus | OrdersController | Checkout, מחזור חיי הזמנה, ניהול פריטי הזמנה וסטטוסים, ופרסום אירועים דרך Kafka Producer |
| **PromotionService** | Promotion | PromotionsController | שירות עצמאי אם המורכבות גבוהה; אחרת מודול Promotions בתוך OrderService |
| **TelemetryService (אופציונלי)** | Rating | ללא Controller ייעודי (middleware-driven) | שירות עצמאי רק אם קיימת לוגיקה עסקית עצמאית; אחרת להשאיר כחלק מה-API/observability |

## שלבי החילוץ

1. **זיהוי גבולות**
   - מפה מלאה של Controller -> Service -> Repository -> Entity.
   - איתור תלות בין תחומים וטבלאות משותפות.
2. **הקמת שלד שירותים**
   - לכל שירות: `API`, `Application`, `Domain`, `Infrastructure`, `Contracts`, `Tests`.
3. **הגדרת חוזי API**
   - חוזים גרסתיים ב-OpenAPI (למשל `/api/v1/orders`).
   - בעלות ברורה על DTOs לכל שירות.
4. **אסטרטגיית פיצול DB**
   - מעבר מ-`StoreContext` משותף ל-DbContext ייעודי לכל שירות.
   - תכנון סדר מיגרציות ותאימות מעבר.
5. **העברת לוגיקה עסקית**
   - העברת הכללים העסקיים מ-`Service/` לשירות היעד.
   - שמירת ה-Controllers הקיימים כ-facade זמני עד cutover.
6. **מודל תקשורת**
   - REST סינכרוני לזרימות שדורשות תשובה מיידית.
   - Kafka לתקשורת אסינכרונית בין שירותים (למשל `OrderCreated`), כאשר ה-Producer נמצא תחת OrderService.
7. **אסטרטגיית בדיקות**
   - שמירת קונבנציות xUnit + Moq + AAA.
   - הוספת contract tests ל-API ולאירועים.
8. **Cutover הדרגתי**
   - Strangler pattern מאחורי Gateway/ניתוב.
   - מעבר לפי bounded context והסרה הדרגתית מהמונוליט.

## הנחיות בעלות על נתונים

- `IdentityService` בעלות על נתוני משתמש/אימות.
- `CatalogService` בעלות על נתוני קטלוג (`Book`, `Author`, `Category`) גם לקריאה וגם לכתיבה בשלב הראשון.
- `OrderService` בעלות על אגרגט הזמנה (`Order`, `OrderItem`, `OrderStatus`) ועל פרסום אירועים אסינכרוניים דרך Kafka Producer.
- `PromotionService` בעלות על נתוני מבצעים אם מופרד; אם לא, הבעלות עוברת למודול Promotions תחת `OrderService`.
- `TelemetryService` (אם מופרד) בעלות על נתוני rating/traffic; אחרת נשאר בתחום ה-API/observability.

כשיש כיום שיתוף נתונים, הגדירו בעלים יחיד וחשפו מידע דרך API או Events בלבד.

## הערות אינטגרציה ופלטפורמה

- שמור JWT לאימות חיצוני.
- לתקשורת פנימית בין שירותים השתמש ב-mTLS או מנגנון שקול.
- שמור אסטרטגיית לוגים מרכזית (NLog/OpenTelemetry).
- שמור Redis במקומות עם עומס קריאה גבוה.
- השאר את Kafka כעמוד שדרה אסינכרוני, כאשר Producer מנוהל מתוך `OrderService` ו-Consumer בפרויקט `OrderLoggerConsumer/`.

## Checklist לפני מיזוג

- [ ] גבול השירות ברור ומנומק.
- [ ] חוזה API גרסתית ומתועד.
- [ ] בעלות נתונים ונתיב מיגרציה מוגדרים.
- [ ] חוזי אירועים ו-policy של retry/DLQ מוגדרים, כולל אחריות Kafka Producer תחת OrderService.
- [ ] Unit, Integration ו-Contract tests כלולים.
- [ ] Health ו-Readiness endpoints מוגדרים.
- [ ] אסטרטגיית CI/CD ו-Docker image מוגדרת.
- [ ] מודל אבטחה (JWT + trust פנימי) מתועד.

## המלצות תפעוליות

- פריסה הדרגתית עם Blue/Green או Canary.
- תצפיתיות מלאה: logs, metrics, traces.
- SLOs והתראות לכל שירות לפני cutover מלא.

## מגבלות חשובות

- אין לבצע Big Bang Rewrite.
- אין להשאיר כתיבה ישירה ל-DB משותף אחרי חילוץ.
- אין להגדיר גבולות רק לפי תיקיות; יש להגדיר לפי בעלות עסקית וקצב שינוי.