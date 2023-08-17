# SampleEfCoreDatabaseRowSizeConsole
簡單計算資料欄大小

## 類別
### SqlFuncHelper
* 放置'user-defined SQL functions', 包含'ColumnDataSize(數值)', 數值為欄位數值/回傳數值的方法(例如ToString)並使用了欄位數值
### CustomRowSizeExtensions
* 組合'ColumnDataSize', 組合方式為'Sum(ColumnDataSize(數值)+ColumnDataSize(數值)+...)'
### SqlServerFuncHelperTranslations / NpgSqlFuncHelperTranslations
* 'user-defined SQL functions'的'Translation'
* TranslationColumnDataSize: 從使用的ColumnDataSize的傳入參數取出ColumnExpression, 並組合成SqlFunctionExpression

## 不同方式的執行結果
![外觀](sample.png)

### 參考
 https://learn.microsoft.com/en-us/ef/core/querying/user-defined-function-mapping
 
