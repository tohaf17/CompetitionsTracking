using System;
using System.Collections.Generic;
using System.Linq;
using CompetitionsTracking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedIfEmpty(CompetitionsTrackingDbContext context)
        {
            Seed(context);
        }

        public static void ClearAndReseed(CompetitionsTrackingDbContext context)
        {
            context.Appeals.RemoveRange(context.Appeals);
            context.Scores.RemoveRange(context.Scores);
            context.SaveChanges();

            context.Results.RemoveRange(context.Results);
            context.SaveChanges();

            context.Entries.RemoveRange(context.Entries);
            context.SaveChanges();

            context.Competitions.RemoveRange(context.Competitions);
            context.SaveChanges();

            context.Users.RemoveRange(context.Users);
            context.SaveChanges();

            context.Judges.RemoveRange(context.Judges);
            context.SaveChanges();

            context.Database.ExecuteSqlRaw("DELETE FROM [team_members]");

            context.Teams.RemoveRange(context.Teams);
            context.SaveChanges();

            context.Persons.RemoveRange(context.Persons);
            context.SaveChanges();

            context.Disciplines.RemoveRange(context.Disciplines);
            context.Apparatuses.RemoveRange(context.Apparatuses);
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();

            ResetIdentity(context, "participants"); 
            ResetIdentity(context, "users");
            ResetIdentity(context, "competitions");
            ResetIdentity(context, "disciplines");
            ResetIdentity(context, "apparatuses");
            ResetIdentity(context, "categories");
            ResetIdentity(context, "entries");
            ResetIdentity(context, "results");
            ResetIdentity(context, "scores");
            ResetIdentity(context, "judges");
            ResetIdentity(context, "appeals");

            Seed(context);
        }


        private static void Seed(CompetitionsTrackingDbContext context)
        {
            var hasher = new PasswordHasher<User>();
            var rnd    = new Random(42); 

            var admin = new User
            {
                Username   = "admin",
                Email      = "admin@gym.ua",
                Role       = UserRole.Admin,
                IsApproved = true,
                CreatedAt  = DateTime.UtcNow
            };
            admin.PasswordHash = hasher.HashPassword(admin, "admin123");

            var coachUser = new User
            {
                Username   = "coach",
                Email      = "coach@gym.ua",
                Role       = UserRole.Trainee,
                IsApproved = true,
                CreatedAt  = DateTime.UtcNow
            };
            coachUser.PasswordHash = hasher.HashPassword(coachUser, "1.q.a.z.");

            var judgeUser1 = new User { Username = "judge1", Email = "j1@gym.ua", Role = UserRole.Guest, IsApproved = true, CreatedAt = DateTime.UtcNow };
            var judgeUser2 = new User { Username = "judge2", Email = "j2@gym.ua", Role = UserRole.Guest, IsApproved = true, CreatedAt = DateTime.UtcNow };

            var guestUser = new User
            {
                Username   = "guest",
                Email      = "guest@gym.ua",
                Role       = UserRole.Guest,
                IsApproved = true,
                CreatedAt  = DateTime.UtcNow
            };
            guestUser.PasswordHash = hasher.HashPassword(guestUser, "guest123");

            context.Users.AddRange(admin, coachUser, judgeUser1, judgeUser2, guestUser);

            var personCoach1 = new Person { Name = "Олена", Surname = "Дерюгіна", Country = "Україна", DateOfBirth = new DateTime(1958, 1, 11, 0, 0, 0, DateTimeKind.Utc), Gender = Gender.Female, Type = "Person" };
            var personCoach2 = new Person { Name = "Ганна", Surname = "Різатдінова", Country = "Україна", DateOfBirth = new DateTime(1993, 7, 16, 0, 0, 0, DateTimeKind.Utc), Gender = Gender.Female, Type = "Person" };
            var personJudge1 = new Person { Name = "Ірина", Surname = "Дерюгіна", Country = "Україна", DateOfBirth = new DateTime(1958, 1, 11, 0, 0, 0, DateTimeKind.Utc), Gender = Gender.Female, Type = "Person" };
            var personJudge2 = new Person { Name = "Наталія", Surname = "Єрьоміна", Country = "Україна", DateOfBirth = new DateTime(1965, 5, 20, 0, 0, 0, DateTimeKind.Utc), Gender = Gender.Female, Type = "Person" };

            coachUser.Person = personCoach1;
            context.Persons.AddRange(personCoach1, personCoach2, personJudge1, personJudge2);
            context.SaveChanges();

            var judge1 = new Judge { PersonId = personJudge1.Id, QualificationLevel = "Міжнародна" };
            var judge2 = new Judge { PersonId = personJudge2.Id, QualificationLevel = "Національна" };
            context.Judges.AddRange(judge1, judge2);
            context.SaveChanges();

            var judgeNames = new[] { "Тетяна", "Ольга", "Марина", "Катерина", "Світлана", "Наталія", "Ірина", "Олена", "Юлія", "Вікторія", "Ганна", "Людмила", "Тетяна", "Оксана", "Лариса" };
            var judgeSurnames = new[] { "Арутюнян", "Шевченко", "Голуб", "Василенко", "Мельник", "Кравчук", "Ковальчук", "Бондаренко", "Сидоренко", "Поліщук", "Бойко", "Мороз", "Лисенко", "Павленко", "Марченко" };
            var judgeLevels = new[] { "Міжнародна", "Національна", "Перша категорія", "Друга категорія", "Міжнародна", "Національна", "Перша категорія", "Друга категорія", "Національна", "Міжнародна", "Перша категорія", "Друга категорія", "Національна", "Перша категорія", "Міжнародна" };

            var newJudgePersons = new List<Person>();
            var newJudges = new List<Judge>();

            for (int i = 0; i < 15; i++)
            {
                var jp = new Person
                {
                    Name = judgeNames[i],
                    Surname = judgeSurnames[i],
                    Country = "Україна",
                    DateOfBirth = new DateTime(1970 + (i % 20), 1 + (i % 12), 1 + (i % 28), 0, 0, 0, DateTimeKind.Utc),
                    Gender = Gender.Female,
                    Type = "Person"
                };
                newJudgePersons.Add(jp);
            }
            context.Persons.AddRange(newJudgePersons);
            context.SaveChanges();

            for (int i = 0; i < 15; i++)
            {
                var j = new Judge
                {
                    PersonId = newJudgePersons[i].Id,
                    QualificationLevel = judgeLevels[i]
                };
                newJudges.Add(j);
            }
            context.Judges.AddRange(newJudges);
            context.SaveChanges();

            var allJudges = new List<Judge> { judge1, judge2 };
            allJudges.AddRange(newJudges);

            var apps = new[] { "Без предмета", "Обруч", "М'яч", "Булави", "Стрічка" }.Select(t => new Apparatus { Type = t }).ToList();
            context.Apparatuses.AddRange(apps);

            var catSenior  = new Category { Type = "Сеньйорки", MinAge = 16, MaxAge = 99 };
            var catJunior  = new Category { Type = "Юніорки",   MinAge = 13, MaxAge = 15 };
            var catYouth   = new Category { Type = "Молодші",     MinAge = 10, MaxAge = 12 };
            context.Categories.AddRange(catSenior, catJunior, catYouth);
            context.SaveChanges();

            var individualDisciplines = apps.Select(app => new Discipline { Type = "Індивідуальна", Apparatus = app }).ToList();
            var groupDisciplines = apps.Select(app => new Discipline { Type = "Групова", Apparatus = app }).ToList();
            var disciplines = new List<Discipline>();
            disciplines.AddRange(individualDisciplines);
            disciplines.AddRange(groupDisciplines);
            context.Disciplines.AddRange(disciplines);
            context.SaveChanges();

             var teams = new List<Team>
            {
                new Team { Name = "Зірки Києва", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Грація Львів", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Дніпро-Гімн", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Одеса-Спорт", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Ніка Харків", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Крок Полтава", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Поділля Хмельницький", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Спартак Чернігів", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Схід Запоріжжя", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Тризуб Івано-Франківськ", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Авангард Ужгород", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Буковина Чернівці", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Авангард Тернопіль", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Грація Рівне", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Спартак Луцьк", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Сокіл Черкаси", Coach = personCoach2, Type = "Team" }
            };
            context.Teams.AddRange(teams);
            context.SaveChanges();

            string[] names = { "Марія", "Софія", "Анна", "Вікторія", "Дарина", "Ольга", "Юлія", "Катерина", "Тетяна", "Олександра", "Христина", "Альона", "Поліна", "Анастасія", "Діана", "Єлизавета", "Ірина", "Надія", "Маргарита", "Валерія", "Оксана", "Яна", "Вероніка", "Світлана" };
            string[] surnames = { "Коваль", "Бондар", "Сидоренко", "Петренко", "Мельник", "Шевченко", "Бойко", "Ткаченко", "Кравченко", "Козак", "Мороз", "Павленко", "Марченко", "Лисенко", "Рудник", "Клименко", "Вовк", "Савченко", "Поліщук", "Гончар", "Карпенко", "Романенко", "Харченко", "Гаврилюк" };

            var athletes = new List<Person>();
            for (int i = 0; i < 24; i++)
            {
                var coach = (i % 2 == 0) ? personCoach1 : personCoach2; 
                var team  = teams[i % teams.Count];
                var dob   = new DateTime(2008 + (i % 4), 1 + (i % 12), 1 + (i % 28), 0, 0, 0, DateTimeKind.Utc);
                var athlete = new Person
                {
                    Name = names[i], Surname = surnames[i], Country = "Україна", DateOfBirth = dob, Gender = Gender.Female, Type = "Person", Mentor = coach
                };
                athletes.Add(athlete);
                team.Members.Add(athlete);
            }
            context.Persons.AddRange(athletes);
            context.SaveChanges();

           
            var competitions = new List<Competition>
            {
                new Competition { Title = "Кубок Дарниці", City = "Київ", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(-28), Status = CompetitionStatus.Finished },
                new Competition { Title = "Київські Грації", City = "Київ", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(2), Status = CompetitionStatus.Ongoing },
                new Competition { Title = "Осінній Листок", City = "Київ", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(20), EndDate = DateTime.UtcNow.AddDays(22), Status = CompetitionStatus.Planned },

                new Competition { Title = "Чемпіонат України", City = "Львів", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(-60), EndDate = DateTime.UtcNow.AddDays(-57), Status = CompetitionStatus.Finished },
                new Competition { Title = "Кубок України", City = "Дніпро", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(3), Status = CompetitionStatus.Ongoing },
                new Competition { Title = "Зимова Казка", City = "Одеса", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(40), EndDate = DateTime.UtcNow.AddDays(43), Status = CompetitionStatus.RegistrationOpen },
                
                new Competition { Title = "Grand Prix Lviv", City = "Львів", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddDays(-90), EndDate = DateTime.UtcNow.AddDays(-87), Status = CompetitionStatus.Finished },
                new Competition { Title = "Kyiv International Cup", City = "Київ", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(4), Status = CompetitionStatus.Ongoing },
                new Competition { Title = "Deriugina Cup", City = "Київ", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddDays(60), EndDate = DateTime.UtcNow.AddDays(65), Status = CompetitionStatus.Planned },

                new Competition { Title = "Весняний Первоцвіт", City = "Хмельницький", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(-15), EndDate = DateTime.UtcNow.AddDays(-13), Status = CompetitionStatus.Finished },
                new Competition { Title = "Золота Стрічка", City = "Чернігів", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(1), Status = CompetitionStatus.Ongoing },
                new Competition { Title = "Кубок Галичини", City = "Львів", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(12), Status = CompetitionStatus.Planned },
                
                new Competition { Title = "Кубок Слобожанщини", City = "Харків", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(-45), EndDate = DateTime.UtcNow.AddDays(-42), Status = CompetitionStatus.Finished },
                new Competition { Title = "Перлина Чорного Моря", City = "Одеса", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(1), EndDate = DateTime.UtcNow.AddDays(4), Status = CompetitionStatus.Ongoing },
                new Competition { Title = "Срібна Обруч", City = "Полтава", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(30), EndDate = DateTime.UtcNow.AddDays(33), Status = CompetitionStatus.RegistrationOpen },
                
                new Competition { Title = "Lviv Open Cup", City = "Львів", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddDays(-75), EndDate = DateTime.UtcNow.AddDays(-72), Status = CompetitionStatus.Finished },
                new Competition { Title = "Carpathian Cup", City = "Ужгород", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddDays(80), EndDate = DateTime.UtcNow.AddDays(85), Status = CompetitionStatus.Planned }
            };
            context.Competitions.AddRange(competitions);
            context.SaveChanges();

            var entries = new List<Entry>();

            foreach (var comp in competitions)
            {
                if (comp.Status == CompetitionStatus.Finished)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        var indDisc = individualDisciplines[i % individualDisciplines.Count];
                        entries.Add(new Entry { Competition = comp, Participant = athletes[i], Discipline = indDisc, Category = catSenior, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = comp.StartDate.AddDays(-14) });
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        var grpDisc = groupDisciplines[i % groupDisciplines.Count];
                        entries.Add(new Entry { Competition = comp, Participant = teams[i], Discipline = grpDisc, Category = catJunior, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = comp.StartDate.AddDays(-14) });
                    }
                }
                else if (comp.Status == CompetitionStatus.Ongoing)
                {
                    for (int i = 12; i < 18; i++)
                    {
                        var indDisc = individualDisciplines[i % individualDisciplines.Count];
                        entries.Add(new Entry { Competition = comp, Participant = athletes[i], Discipline = indDisc, Category = catJunior, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Active, SubmittedAt = comp.StartDate.AddDays(-14) });
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        var grpDisc = groupDisciplines[i % groupDisciplines.Count];
                        entries.Add(new Entry { Competition = comp, Participant = teams[i], Discipline = grpDisc, Category = catJunior, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Active, SubmittedAt = comp.StartDate.AddDays(-14) });
                    }
                }
                else
                {
                    for (int i = 18; i < 24; i++)
                    {
                        var indDisc = individualDisciplines[i % individualDisciplines.Count];
                        entries.Add(new Entry { Competition = comp, Participant = athletes[i], Discipline = indDisc, Category = catYouth, ApplicationStatus = (i % 2 == 0) ? ApplicationStatus.Pending : ApplicationStatus.Accepted, EntryStatus = EntryStatus.Registered, SubmittedAt = DateTime.UtcNow.AddDays(-2) });
                    }
                }
            }
            context.Entries.AddRange(entries);
            context.SaveChanges();

            var finishedEntries = entries.Where(e => e.EntryStatus == EntryStatus.Finished).ToList();
            foreach (var entry in finishedEntries)
            {
                float d     = 8.0f + (float)(rnd.NextDouble() * 4.0);
                float a     = 6.0f + (float)(rnd.NextDouble() * 2.5);
                float ex    = 6.0f + (float)(rnd.NextDouble() * 2.5);
                float final = (float)Math.Round(d + a + ex, 3);

                var shuffledJudges = allJudges.OrderBy(x => rnd.Next()).Take(3).ToList();
                var jD = shuffledJudges[0];
                var jA = shuffledJudges[1];
                var jE = shuffledJudges[2];

                int jumpCount = rnd.Next(2, 5); 
                int balanceCount = rnd.Next(2, 5); 
                int rotationCount = rnd.Next(2, 5); 
                int dynamicRotationCount = rnd.Next(1, 4);
                int elementCount = jumpCount + balanceCount + rotationCount + dynamicRotationCount;

                context.Results.Add(new Result { Entry = entry, FinalScore = final, Place = 0, AwardedMedal = "" });
                context.Scores.Add(new Score 
                { 
                    Entry = entry, 
                    Judge = jD, 
                    Type = ScoreType.D, 
                    ScoreValue = d, 
                    JumpCount = jumpCount,
                    BalanceCount = balanceCount,
                    RotationCount = rotationCount,
                    DynamicRotationCount = dynamicRotationCount,
                    ElementCount = elementCount 
                });
                context.Scores.Add(new Score 
                { 
                    Entry = entry, 
                    Judge = jA, 
                    Type = ScoreType.A, 
                    ScoreValue = a,
                    JumpCount = 0,
                    BalanceCount = 0,
                    RotationCount = 0,
                    DynamicRotationCount = 0,
                    ElementCount = 0
                });
                context.Scores.Add(new Score 
                { 
                    Entry = entry, 
                    Judge = jE, 
                    Type = ScoreType.E, 
                    ScoreValue = ex,
                    JumpCount = 0,
                    BalanceCount = 0,
                    RotationCount = 0,
                    DynamicRotationCount = 0,
                    ElementCount = 0
                });
            }
            context.SaveChanges();

            var allResults = context.Results.Include(r => r.Entry).ToList();

            foreach (var compGroup in allResults.GroupBy(r => r.Entry.CompetitionId))
            {
                foreach (var group in compGroup.GroupBy(r => new { r.Entry.DisciplineId, r.Entry.CategoryId }))
                {
                    var sorted = group.OrderByDescending(r => r.FinalScore).ToList();
                    for (int i = 0; i < sorted.Count; i++)
                    {
                        sorted[i].Place = i + 1;
                        sorted[i].AwardedMedal = i == 0 ? "Золото" : i == 1 ? "Срібло" : i == 2 ? "Бронза" : "";
                    }
                }
            }
            context.SaveChanges();

            var allResultsForAppeals = context.Results.Include(r => r.Entry).ToList();
            var appealReasons = new[]
            {
                "Некоректно зараховано складність тіла (DB). Прошу переглянути відео.",
                "Помилка в оцінці за артистизм.",
                "Не зафіксовано ризик.",
                "Не враховано динамічний елемент з обертанням (R).",
                "Некоректно оцінено виконання труднощів предмета (DA).",
                "Занижено оцінку за танцювальні доріжки.",
                "Невірно пораховано кількість обертань у піруеті.",
                "Прохання переглянути збавку за втрату предмета поза майданчиком.",
                "Запит на перегляд оцінки за трудність тіла (стрибок шагом).",
                "Помилка при підрахунку вартості комбінації хвиль.",
                "Прохання переглянути збавку за вихід за межі майданчика.",
                "Оцінка за артистизм не відповідає складності музичного супроводу.",
                "Не зараховано ловіння без зорового контролю.",
                "Помилка в оцінці трудності зв'язки елементів рівноваги.",
                "Заниження оцінки за технічне виконання оригінального елемента.",
                "Помилка в нарахуванні збавки за синхронність у груповій вправі.",
                "Некоректна фіксація тривалості статичної рівноваги."
            };

            for (int i = 0; i < Math.Min(appealReasons.Length, allResultsForAppeals.Count); i++)
            {
                var result = allResultsForAppeals[i];
                var reason = appealReasons[i];
                var status = (AppealStatus)(i % 3); 
                var createdAt = DateTime.UtcNow.AddDays(-10 + i);

                var appeal = new Appeal
                {
                    Result = result,
                    Reason = reason,
                    Status = status,
                    CreatedAt = createdAt
                };

                if (status != AppealStatus.Pending)
                {
                    appeal.ResolvedAt = createdAt.AddHours(2 + (i % 5));
                }

                context.Appeals.Add(appeal);
            }
            context.SaveChanges();
        }

        private static void ResetIdentity(CompetitionsTrackingDbContext context, string tableName)
        {
            try
            {
                context.Database.ExecuteSqlRaw(
                    $"IF EXISTS (SELECT 1 FROM sys.tables WHERE name = '{tableName}') " +
                    $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)");
            }
            catch
            {
        
            }
        }
    }
}
