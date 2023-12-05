# Starkov
Требования к системе: наличие установленного .NET SDK 6 и выше, установленная Postgres<br>
Как запустить:
1. Склонировать репозиторий
2. Сбилдить в release режиме
3. Запустить полученный exe файл (миграции в БД накатятся самостоятельно при первом запуске) через cmd, так же передать одну из команд

Доступные команды:<br>
- import -p path -t type - импорт файла в БД<br>
  -p path - путь до tsv файла<br>
  -t type - тип импорта. d - отделы, e - сотрудники, j - должность<br>
  Пример: Starkov.Console import -p C:\departments.tsv -t d<br>
  
- output -id id<br>
  -id <id> - **опциональный параметр**, id отдела<br>
  Пример: Starkov.Console output -id 22<br>
  Пример: Starkov.Console output<br>
  
