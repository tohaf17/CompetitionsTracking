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
            if (context.Users.Any() || context.Competitions.Any() || context.Judges.Any())
            {
                EnsureCompetitionTypeSamples(context);
                EnsureAtLeastOneFinishedPerLevel(context);
                EnsureResultsForFinishedCompetitions(context);
                return;
            }

            
            context.Database.ExecuteSqlRaw("DELETE FROM team_members");
            context.Appeals.RemoveRange(context.Appeals);
            context.Results.RemoveRange(context.Results);
            context.Scores.RemoveRange(context.Scores);
            context.Entries.RemoveRange(context.Entries);
            context.Judges.RemoveRange(context.Judges);
            context.Competitions.RemoveRange(context.Competitions);
            context.Disciplines.RemoveRange(context.Disciplines);
            context.Categories.RemoveRange(context.Categories);
            context.Apparatuses.RemoveRange(context.Apparatuses);
            context.Teams.RemoveRange(context.Teams);
            
            var personsWithMentors = context.Persons.Where(p => p.MentorId != null).ToList();
            foreach (var p in personsWithMentors) p.MentorId = null;
            context.SaveChanges();

            context.Participants.RemoveRange(context.Participants);
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();

            var hasher = new PasswordHasher<User>();

            var admin = new User { Username = "admin", Email = "admin@example.com", Role = UserRole.Admin, IsApproved = true, CreatedAt = DateTime.UtcNow };
            admin.PasswordHash = hasher.HashPassword(admin, "admin123");

            var trainee = new User { Username = "trainee", Email = "trainee@example.com", Role = UserRole.Trainee, IsApproved = true, CreatedAt = DateTime.UtcNow };
            trainee.PasswordHash = hasher.HashPassword(trainee, "trainee123");

            var guest = new User { Username = "guest", Email = "guest@example.com", Role = UserRole.Guest, IsApproved = true, CreatedAt = DateTime.UtcNow };
            guest.PasswordHash = hasher.HashPassword(guest, "guest123");

            context.Users.AddRange(admin, trainee, guest);

            
            var coachKyiv = new Person { Name = "Олена", Surname = "Дерюгіна", Country = "Україна", DateOfBirth = new DateTime(1958, 2, 11).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachLviv = new Person { Name = "Альона", Surname = "Петренко", Country = "Україна", DateOfBirth = new DateTime(1985, 5, 20).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachKharkiv = new Person { Name = "Світлана", Surname = "Медведєва", Country = "Україна", DateOfBirth = new DateTime(1980, 10, 5).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachOdesa = new Person { Name = "Наталія", Surname = "Горбань", Country = "Україна", DateOfBirth = new DateTime(1978, 3, 12).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachDnipro = new Person { Name = "Ірина", Surname = "Савченко", Country = "Україна", DateOfBirth = new DateTime(1982, 11, 30).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };

            trainee.Person = coachKyiv;

            context.Persons.AddRange(coachKyiv, coachLviv, coachKharkiv, coachOdesa, coachDnipro);

            var athleteK1 = new Person { Name = "Марія", Surname = "Коваль", Country = "Україна", DateOfBirth = new DateTime(2008, 3, 15).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKyiv };
            var athleteK2 = new Person { Name = "Софія", Surname = "Бондар", Country = "Україна", DateOfBirth = new DateTime(2009, 7, 22).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKyiv };
            var athleteK3 = new Person { Name = "Поліна", Surname = "Шевченко", Country = "Україна", DateOfBirth = new DateTime(2010, 1, 10).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKyiv };
            var athleteK4 = new Person { Name = "Анна", Surname = "Лисенко", Country = "Україна", DateOfBirth = new DateTime(2011, 11, 3).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKyiv };

            var athleteL1 = new Person { Name = "Вікторія", Surname = "Мельник", Country = "Україна", DateOfBirth = new DateTime(2012, 6, 18).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachLviv };
            var athleteL2 = new Person { Name = "Катерина", Surname = "Ткаченко", Country = "Україна", DateOfBirth = new DateTime(2007, 9, 25).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachLviv };
            var athleteL3 = new Person { Name = "Анастасія", Surname = "Мороз", Country = "Україна", DateOfBirth = new DateTime(2008, 5, 5).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachLviv };
            var athleteL4 = new Person { Name = "Юлія", Surname = "Федоренко", Country = "Україна", DateOfBirth = new DateTime(2009, 12, 12).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachLviv };

            var athleteH1 = new Person { Name = "Олена", Surname = "Бойко", Country = "Україна", DateOfBirth = new DateTime(2010, 4, 1).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKharkiv };
            var athleteH2 = new Person { Name = "Дарина", Surname = "Сергієнко", Country = "Україна", DateOfBirth = new DateTime(2011, 8, 15).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKharkiv };
            var athleteH3 = new Person { Name = "Наталія", Surname = "Кравченко", Country = "Україна", DateOfBirth = new DateTime(2012, 2, 28).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKharkiv };
            var athleteH4 = new Person { Name = "Ірина", Surname = "Мовчан", Country = "Україна", DateOfBirth = new DateTime(2009, 10, 10).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKharkiv };

            var athleteO1 = new Person { Name = "Тетяна", Surname = "Поліщук", Country = "Україна", DateOfBirth = new DateTime(2008, 1, 20).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachOdesa };
            var athleteO2 = new Person { Name = "Оксана", Surname = "Коваленко", Country = "Україна", DateOfBirth = new DateTime(2009, 2, 11).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachOdesa };
            var athleteO3 = new Person { Name = "Людмила", Surname = "Петренко", Country = "Україна", DateOfBirth = new DateTime(2010, 6, 30).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachOdesa };
            var athleteO4 = new Person { Name = "Ольга", Surname = "Сидоренко", Country = "Україна", DateOfBirth = new DateTime(2011, 9, 5).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachOdesa };

            var athleteD1 = new Person { Name = "Світлана", Surname = "Іванова", Country = "Україна", DateOfBirth = new DateTime(2007, 7, 7).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachDnipro };
            var athleteD2 = new Person { Name = "Олена", Surname = "Кузнєцова", Country = "Україна", DateOfBirth = new DateTime(2008, 8, 8).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachDnipro };
            var athleteD3 = new Person { Name = "Марина", Surname = "Павлова", Country = "Україна", DateOfBirth = new DateTime(2009, 9, 9).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachDnipro };
            var athleteD4 = new Person { Name = "Алла", Surname = "Васильєва", Country = "Україна", DateOfBirth = new DateTime(2010, 10, 10).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachDnipro };

            var allAthletes = new List<Person> {
                athleteK1, athleteK2, athleteK3, athleteK4,
                athleteL1, athleteL2, athleteL3, athleteL4,
                athleteH1, athleteH2, athleteH3, athleteH4,
                athleteO1, athleteO2, athleteO3, athleteO4,
                athleteD1, athleteD2, athleteD3, athleteD4
            };
            context.Persons.AddRange(allAthletes);

            var teamKyiv = new Team { Name = "Зірки Києва", Coach = coachKyiv, Members = new List<Person> { athleteK1, athleteK2 }, Type = "Team" };
            var teamLviv = new Team { Name = "Грація Львів", Coach = coachLviv, Members = new List<Person> { athleteL1, athleteL2 }, Type = "Team" };
            var teamKharkiv = new Team { Name = "Олімп Харків", Coach = coachKharkiv, Members = new List<Person> { athleteH1, athleteH2 }, Type = "Team" };
            var teamOdesa = new Team { Name = "Чорноморські Перлини", Coach = coachOdesa, Members = new List<Person> { athleteO1, athleteO2 }, Type = "Team" };
            var teamDnipro = new Team { Name = "Дніпровська Хвиля", Coach = coachDnipro, Members = new List<Person> { athleteD1, athleteD2 }, Type = "Team" };

            context.Teams.AddRange(teamKyiv, teamLviv, teamKharkiv, teamOdesa, teamDnipro);

            var appFloor = new Apparatus { Type = "Без предмета" };
            var appHoop = new Apparatus { Type = "Обруч" };
            var appBall = new Apparatus { Type = "М'яч" };
            var appClubs = new Apparatus { Type = "Булави" };
            var appRibbon = new Apparatus { Type = "Стрічка" };
            var appRope = new Apparatus { Type = "Скакалка" };

            context.Apparatuses.AddRange(appFloor, appHoop, appBall, appClubs, appRibbon, appRope);

            var discFloor = new Discipline { Type = "Індивідуальна вправа (Без предмета)", Apparatus = appFloor };
            var discHoop = new Discipline { Type = "Індивідуальна вправа (Обруч)", Apparatus = appHoop };
            var discBall = new Discipline { Type = "Індивідуальна вправа (М'яч)", Apparatus = appBall };
            var discClubs = new Discipline { Type = "Індивідуальна вправа (Булави)", Apparatus = appClubs };
            var discRibbon = new Discipline { Type = "Індивідуальна вправа (Стрічка)", Apparatus = appRibbon };

            context.Disciplines.AddRange(discFloor, discHoop, discBall, discClubs, discRibbon);

            var catSeniors = new Category { Type = "Сеньйорки", MinAge = 16, MaxAge = 99 };
            var catJuniors = new Category { Type = "Юніорки", MinAge = 13, MaxAge = 15 };
            var catPreJuniors = new Category { Type = "Пре-юніорки", MinAge = 10, MaxAge = 12 };

            context.Categories.AddRange(catSeniors, catJuniors, catPreJuniors);

            var comp1 = new Competition { Title = "Чемпіонат України 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddMonths(-3), EndDate = DateTime.UtcNow.AddMonths(-3).AddDays(5), Status = CompetitionStatus.Finished };
            var comp2 = new Competition { Title = "Кубок Львова 2026", City = "Львів", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddMonths(-2), EndDate = DateTime.UtcNow.AddMonths(-2).AddDays(3), Status = CompetitionStatus.Finished };
            var comp3 = new Competition { Title = "Одеська Осінь 2026", City = "Одеса", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddMonths(4), EndDate = DateTime.UtcNow.AddMonths(4).AddDays(4), Status = CompetitionStatus.Planned };
            var comp4 = new Competition { Title = "Дніпровські Зорі 2026", City = "Дніпро", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddMonths(6), EndDate = DateTime.UtcNow.AddMonths(6).AddDays(3), Status = CompetitionStatus.Planned };
            var comp5 = new Competition { Title = "Кубок України 2026", City = "Харків", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddMonths(8), EndDate = DateTime.UtcNow.AddMonths(8).AddDays(4), Status = CompetitionStatus.Planned };
            var comp6 = new Competition { Title = "Всеукраїнська першість 2026", City = "Черкаси", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddMonths(9), EndDate = DateTime.UtcNow.AddMonths(9).AddDays(3), Status = CompetitionStatus.Planned };
            var comp7 = new Competition { Title = "Kyiv International RG Cup 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = DateTime.UtcNow.AddMonths(-1).AddDays(4), Status = CompetitionStatus.Finished };
            var comp8 = new Competition { Title = "Warsaw Spring Invitational 2026", City = "Варшава", Country = "Польща", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(11), EndDate = DateTime.UtcNow.AddMonths(11).AddDays(3), Status = CompetitionStatus.Planned };
            var comp9 = new Competition { Title = "Prague Stars Grand Prix 2026", City = "Прага", Country = "Чехія", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(12), EndDate = DateTime.UtcNow.AddMonths(12).AddDays(4), Status = CompetitionStatus.Planned };

            context.Competitions.AddRange(comp1, comp2, comp3, comp4, comp5, comp6, comp7, comp8, comp9);

            var jP1 = new Person { Name = "Наталія", Surname = "Степанова", Country = "Україна", DateOfBirth = new DateTime(1965, 4, 12).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var jP2 = new Person { Name = "Олександра", Surname = "Біла", Country = "Україна", DateOfBirth = new DateTime(1972, 8, 30).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var jP3 = new Person { Name = "Тетяна", Surname = "Чорна", Country = "Україна", DateOfBirth = new DateTime(1975, 1, 15).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var jP4 = new Person { Name = "Олена", Surname = "Синиця", Country = "Україна", DateOfBirth = new DateTime(1980, 5, 20).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };

            context.Persons.AddRange(jP1, jP2, jP3, jP4);

            var judge1 = new Judge { Person = jP1, QualificationLevel = "Міжнародна" };
            var judge2 = new Judge { Person = jP2, QualificationLevel = "Міжнародна" };
            var judge3 = new Judge { Person = jP3, QualificationLevel = "Національна" };
            var judge4 = new Judge { Person = jP4, QualificationLevel = "Перша категорія" };

            context.Judges.AddRange(judge1, judge2, judge3, judge4);

            var entries = new List<Entry>();

            foreach (var athlete in allAthletes)
            {
                entries.Add(new Entry 
                { 
                    Competition = comp1, Participant = athlete, Discipline = discHoop, Category = catJuniors, 
                    ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-4) 
                });

                entries.Add(new Entry 
                { 
                    Competition = comp2, Participant = athlete, Discipline = discBall, Category = catJuniors, 
                    ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-3) 
                });

                entries.Add(new Entry 
                { 
                    Competition = comp7, Participant = athlete, Discipline = discClubs, Category = catJuniors, 
                    ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-2) 
                });
                
                entries.Add(new Entry 
                { 
                    Competition = comp3, Participant = athlete, Discipline = discClubs, Category = catJuniors, 
                    ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Registered, SubmittedAt = DateTime.UtcNow.AddDays(-5) 
                });
            }

            var teamEntries = new List<Entry>
            {
                new Entry { Competition = comp1, Participant = teamKyiv, Discipline = discHoop, Category = catSeniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-4) },
                new Entry { Competition = comp2, Participant = teamLviv, Discipline = discBall, Category = catSeniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Active, SubmittedAt = DateTime.UtcNow.AddDays(-10) },
                new Entry { Competition = comp3, Participant = teamKharkiv, Discipline = discClubs, Category = catSeniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Registered, SubmittedAt = DateTime.UtcNow.AddDays(-5) }
            };

            context.Entries.AddRange(entries);
            context.Entries.AddRange(teamEntries);

            
            for (int i = 0; i < 5; i++)
            {
                var entry = entries[i * 4];
                context.Scores.AddRange(
                    new Score { Entry = entry, Judge = judge1, Type = ScoreType.D, ScoreValue = 12.0f + i * 0.5f },
                    new Score { Entry = entry, Judge = judge2, Type = ScoreType.A, ScoreValue = 8.0f + i * 0.1f },
                    new Score { Entry = entry, Judge = judge3, Type = ScoreType.E, ScoreValue = 7.5f + i * 0.2f }
                );
                
                var result = new Result { Entry = entry, Place = i + 1, FinalScore = 27.5f + i * 0.8f, AwardedMedal = i == 0 ? "Золото" : (i == 1 ? "Срібло" : (i == 2 ? "Бронза" : "")) };
                context.Results.Add(result);
            }

            for (int i = 0; i < 5; i++)
            {
                var entry = entries[i * 4 + 1]; 
                context.Scores.AddRange(
                    new Score { Entry = entry, Judge = judge1, Type = ScoreType.D, ScoreValue = 10.0f + i * 0.3f },
                    new Score { Entry = entry, Judge = judge2, Type = ScoreType.A, ScoreValue = 7.0f + i * 0.1f },
                    new Score { Entry = entry, Judge = judge3, Type = ScoreType.E, ScoreValue = 6.5f + i * 0.2f }
                );
                
                var result = new Result { Entry = entry, Place = i + 1, FinalScore = 23.5f + i * 0.6f, AwardedMedal = i == 0 ? "Золото" : (i == 1 ? "Срібло" : (i == 2 ? "Бронза" : "")) };
                context.Results.Add(result);
            }

            for (int i = 0; i < 5; i++)
            {
                var entry = entries[i * 4 + 2];
                context.Scores.AddRange(
                    new Score { Entry = entry, Judge = judge1, Type = ScoreType.D, ScoreValue = 15.0f + i * 0.4f },
                    new Score { Entry = entry, Judge = judge2, Type = ScoreType.A, ScoreValue = 9.0f + i * 0.1f },
                    new Score { Entry = entry, Judge = judge3, Type = ScoreType.E, ScoreValue = 8.5f + i * 0.2f }
                );
                
                var result = new Result { Entry = entry, Place = i + 1, FinalScore = 32.5f + i * 0.7f, AwardedMedal = i == 0 ? "Золото" : (i == 1 ? "Срібло" : (i == 2 ? "Бронза" : "")) };
                context.Results.Add(result);
            }

            if (context.Results.Local.Count > 1)
            {
                var resultForAppeal = context.Results.Local.ElementAt(1);
                var appeal = new Appeal { Result = resultForAppeal, Reason = "Незгода з оцінкою за виконання (E)", Status = AppealStatus.Rejected, CreatedAt = DateTime.UtcNow.AddMonths(-3).AddDays(6), ResolvedAt = DateTime.UtcNow.AddMonths(-3).AddDays(7) };
                context.Appeals.Add(appeal);
            }

            context.SaveChanges();
        }

        private static void EnsureCompetitionTypeSamples(CompetitionsTrackingDbContext context)
        {
            var sampleCompetitions = new List<Competition>
            {
                new Competition { Title = "Кубок Львова 2026", City = "Львів", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddMonths(-2), EndDate = DateTime.UtcNow.AddMonths(-2).AddDays(3), Status = CompetitionStatus.Finished },
                new Competition { Title = "Одеська Осінь 2026", City = "Одеса", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddMonths(4), EndDate = DateTime.UtcNow.AddMonths(4).AddDays(4), Status = CompetitionStatus.Planned },
                new Competition { Title = "Дніпровські Зорі 2026", City = "Дніпро", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddMonths(6), EndDate = DateTime.UtcNow.AddMonths(6).AddDays(3), Status = CompetitionStatus.Planned },
                new Competition { Title = "Чемпіонат України 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddMonths(-3), EndDate = DateTime.UtcNow.AddMonths(-3).AddDays(5), Status = CompetitionStatus.Finished },
                new Competition { Title = "Кубок України 2026", City = "Харків", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddMonths(8), EndDate = DateTime.UtcNow.AddMonths(8).AddDays(4), Status = CompetitionStatus.Planned },
                new Competition { Title = "Всеукраїнська першість 2026", City = "Черкаси", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddMonths(9), EndDate = DateTime.UtcNow.AddMonths(9).AddDays(3), Status = CompetitionStatus.Planned },
                new Competition { Title = "Kyiv International RG Cup 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = DateTime.UtcNow.AddMonths(-1).AddDays(4), Status = CompetitionStatus.Finished },
                new Competition { Title = "Warsaw Spring Invitational 2026", City = "Варшава", Country = "Польща", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(11), EndDate = DateTime.UtcNow.AddMonths(11).AddDays(3), Status = CompetitionStatus.Planned },
                new Competition { Title = "Prague Stars Grand Prix 2026", City = "Прага", Country = "Чехія", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(12), EndDate = DateTime.UtcNow.AddMonths(12).AddDays(4), Status = CompetitionStatus.Planned }
            };

            var sampleTitles = sampleCompetitions.Select(s => s.Title).ToList();
            var existingByTitle = context.Competitions
                .Where(c => sampleTitles.Contains(c.Title))
                .ToDictionary(c => c.Title);

            foreach (var sample in sampleCompetitions)
            {
                if (existingByTitle.TryGetValue(sample.Title, out var existing))
                {
                    existing.Level = sample.Level;
                    existing.Country = sample.Country;
                    existing.Status = sample.Status;
                    existing.StartDate = sample.StartDate;
                    existing.EndDate = sample.EndDate;
                    existing.City = sample.City;
                    continue;
                }

                context.Competitions.Add(sample);
            }

            context.SaveChanges();
        }

        private static void EnsureResultsForFinishedCompetitions(CompetitionsTrackingDbContext context)
        {
            var finishedComps = context.Competitions
                .Where(c => c.Status == CompetitionStatus.Finished)
                .Include(c => c.Entries)
                .ToList();

            if (!finishedComps.Any()) return;

            var athletes = context.Participants.OfType<Person>().Take(10).ToList();
            var judges = context.Judges.Include(j => j.Person).Take(4).ToList();
            var disc = context.Disciplines.FirstOrDefault();
            var cat = context.Categories.FirstOrDefault();

            if (!athletes.Any() || !judges.Any() || disc == null || cat == null) return;

            foreach (var comp in finishedComps)
            {
                if (context.Results.Any(r => r.Entry.CompetitionId == comp.Id)) continue;

                for (int i = 0; i < Math.Min(athletes.Count, 5); i++)
                {
                    var athlete = athletes[i];
                    
                    var entry = context.Entries.FirstOrDefault(e => e.CompetitionId == comp.Id && e.ParticipantId == athlete.Id);
                    if (entry == null)
                    {
                        entry = new Entry 
                        { 
                            Competition = comp, Participant = athlete, Discipline = disc, Category = cat, 
                            ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-5) 
                        };
                        context.Entries.Add(entry);
                    }
                    else
                    {
                        entry.EntryStatus = EntryStatus.Finished;
                    }

                    if (!context.Scores.Any(s => s.EntryId == entry.Id))
                    {
                        if (judges.Count >= 3)
                        {
                            context.Scores.Add(new Score { Entry = entry, Judge = judges[0], Type = ScoreType.D, ScoreValue = 12.0f + i * 0.5f });
                            context.Scores.Add(new Score { Entry = entry, Judge = judges[1], Type = ScoreType.A, ScoreValue = 8.0f + i * 0.1f });
                            context.Scores.Add(new Score { Entry = entry, Judge = judges[2], Type = ScoreType.E, ScoreValue = 7.5f + i * 0.2f });
                        }
                    }

                    if (!context.Results.Any(r => r.EntryId == entry.Id))
                    {
                        context.Results.Add(new Result 
                        { 
                            Entry = entry, 
                            Place = i + 1, 
                            FinalScore = 28.0f + i * 0.5f, 
                            AwardedMedal = i == 0 ? "Золото" : (i == 1 ? "Срібло" : (i == 2 ? "Бронза" : "")) 
                        });
                    }
                }
            }

            context.SaveChanges();
        }
        private static void EnsureAtLeastOneFinishedPerLevel(CompetitionsTrackingDbContext context)
        {
            var levels = Enum.GetValues(typeof(CompetitionLevel)).Cast<CompetitionLevel>();

            foreach (var level in levels)
            {
                if (!context.Competitions.Any(c => c.Level == level && c.Status == CompetitionStatus.Finished))
                {
                    var comp = context.Competitions.FirstOrDefault(c => c.Level == level);
                    if (comp != null)
                    {
                        comp.Status = CompetitionStatus.Finished;
                        comp.StartDate = DateTime.UtcNow.AddMonths(-1);
                        comp.EndDate = DateTime.UtcNow.AddMonths(-1).AddDays(3);
                    }
                }
            }
            context.SaveChanges();
        }
    }
}
