Это программа-клиент для просмотра следующих данных: списки сотрудников, 
статистическая информация по спискам сотрудников в днях.

Подключение к серверу (имя сервера, база, логин, пароль) вынесены в 
настроечный файл .config. Для изменения строки подключения введите
нужную строку в ковычках после "СonnectionString =".
В файле EmployeeDB.mdf хранится БД с необходимыми таблицами и процедурами

Программа имеет следующие функции:
1.Выводит в списке сотрудников следующие данные: ФИО (в формате Фамилия И. О.), 
наименование статуса, наименование отдела, наименование должности, даты приема 
и увольнения если есть. Предусмотреть возможность сортировки списка по любому 
из полей. Осущетсвляет фильтрацию списка по статусу, отделу, должности и части
фамилии (по вхождению заданной строки в фамилию).

2.Выводит следующую статистику по списку сотрудников: количество сотрудников
выбранного статуса, принятых или уволенных на работу за заданный период с 
разбиением по дням. 



Таблицы:

dbo.status – справочник статусов 

CREATE TABLE [dbo].[status](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_status] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

dbo.post – справочник должностей 

CREATE TABLE [dbo].[posts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_posts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

dbo.deps – справочник отделов 
CREATE TABLE [dbo].[deps](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_deps] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

dbo.persons – журнал сотрудников (ФИО, дата приема и дата увольнения (могут быть не заполнены), статус, отдел и должность)
CREATE TABLE [dbo].[persons](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [varchar](100) NOT NULL,
	[second_name] [varchar](100) NOT NULL,
	[last_name] [varchar](100) NOT NULL,
	[date_employ] [datetime] NULL,
	[date_uneploy] [datetime] NULL,
	[status] [int] NOT NULL,
	[id_dep] [int] NOT NULL,
	[id_post] [int] NOT NULL,
 CONSTRAINT [PK_persons] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


Хранимые процедуры:

CREATE PROCEDURE [dbo].[Pr_DepsNames]
AS
	SELECT [name] FROM [deps]
RETURN 0

CREATE PROCEDURE [dbo].[Pr_PostsNames]
AS
	SELECT [name] FROM [posts]
RETURN 0

CREATE PROCEDURE [dbo].[Pr_StatusNames]
AS
	SELECT [name] FROM [status]
RETURN 0

CREATE PROCEDURE [dbo].[Pr_EmployStats]
	@Date1 DateTime,
	@Date2 DateTime,
	@Status VARCHAR(100)
AS
	SELECT [date_employ] AS 'Дата Устройства', [second_name] AS N'Фамилия', [first_name] AS N'Имя', [last_name] AS N'Отчество',
	[status].[name] AS N'Статус', [deps].[name] AS N'Отдел', [posts].[name] AS N'Должность'
	FROM [persons]
		INNER JOIN [status] ON [persons].[status] = [status].[id]
		INNER JOIN [deps] ON [persons].[id_dep] = [deps].[id]
		INNER JOIN [posts] ON [persons].[id_post] = [posts].[id]
		WHERE [date_employ] BETWEEN @Date1 AND @Date2 
			AND [status].[name] = @Status
			ORDER BY [date_employ]
RETURN 0

CREATE PROCEDURE [dbo].[Pr_UneployStats]
	@Date1 DateTime,
	@Date2 DateTime,
	@Status VARCHAR(100)
AS
	SELECT [date_uneploy] AS 'Дата Увольнения', [second_name] AS N'Фамилия', [first_name] AS N'Имя', [last_name] AS N'Отчество',
	[status].[name] AS N'Статус', [deps].[name] AS N'Отдел', [posts].[name] AS N'Должность'
	FROM [persons]
		INNER JOIN [status] ON [persons].[status] = [status].[id]
		INNER JOIN [deps] ON [persons].[id_dep] = [deps].[id]
		INNER JOIN [posts] ON [persons].[id_post] = [posts].[id]
		WHERE [date_uneploy] BETWEEN @Date1 AND @Date2 
			AND [status].[name] = @Status
			ORDER BY [date_uneploy]
RETURN 0

CREATE PROCEDURE [dbo].[Pr_Select]
@PersonName VARCHAR(100),
@PersonStatus VARCHAR(100),
@PersonDep VARCHAR(100),
@PersonPost VARCHAR(100)
AS
	SELECT [second_name] AS N'Фамилия', [first_name] AS N'Имя', [last_name] AS N'Отчество',
	[status].[name] AS N'Статус', [deps].[name] AS N'Отдел', [posts].[name] AS N'Должность',
	[date_employ] AS N'Дата устройства', [date_uneploy] AS N'Дата увольнения'
	FROM [persons]
	INNER JOIN [status] ON [persons].[status] = [status].[id]
	INNER JOIN [deps] ON [persons].[id_dep] = [deps].[id]
	INNER JOIN [posts] ON [persons].[id_post] = [posts].[id]
		WHERE [second_name] LIKE '%' + @PersonName + '%' 
			AND [status].[name] = CASE WHEN @PersonStatus != '' THEN @PersonStatus ELSE [status].[name] END
			AND [posts].[name] = CASE WHEN @PersonPost != '' THEN @PersonPost ELSE [posts].[name] END
			AND [deps].[name] = CASE WHEN @PersonDep != '' THEN @PersonDep ELSE [deps].[name] END
RETURN 0
