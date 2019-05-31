
SET timeout=2


start cmd /k call StartCashCard.Service.bat

timeout %timeout%

start cmd /k call StartSimpleVirtualATMMachine.bat




