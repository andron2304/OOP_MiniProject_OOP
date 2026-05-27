#!/bin/bash
# MusicPlayer_OOP - Скрипт налаштування Clean Architecture
# Створює структуру рішення з проектами Domain, Application, Infrastructure, Console та Tests

set -e

echo "Створення структури рішення MusicPlayer_OOP..."

# Створити рішення
dotnet new sln -n MusicPlayer

# Створити проект Domain
dotnet new classlib -n MusicPlayer.Domain -o src/MusicPlayer.Domain
dotnet sln MusicPlayer.sln add src/MusicPlayer.Domain/MusicPlayer.Domain.csproj

# Створити проект Application
dotnet new classlib -n MusicPlayer.Application -o src/MusicPlayer.Application
dotnet sln MusicPlayer.sln add src/MusicPlayer.Application/MusicPlayer.Application.csproj

# Створити проект Infrastructure
dotnet new classlib -n MusicPlayer.Infrastructure -o src/MusicPlayer.Infrastructure
dotnet sln MusicPlayer.sln add src/MusicPlayer.Infrastructure/MusicPlayer.Infrastructure.csproj

# Створити проект Console
dotnet new console -n MusicPlayer.Console -o src/MusicPlayer.Console
dotnet sln MusicPlayer.sln add src/MusicPlayer.Console/MusicPlayer.Console.csproj

# Створити проект Tests
dotnet new nunit -n MusicPlayer.Tests -o tests/MusicPlayer.Tests
dotnet sln MusicPlayer.sln add tests/MusicPlayer.Tests/MusicPlayer.Tests.csproj

# Додати посилання на проекти
echo "Додавання посилань на проекти..."

# Application посилається на Domain
dotnet add src/MusicPlayer.Application/MusicPlayer.Application.csproj reference src/MusicPlayer.Domain/MusicPlayer.Domain.csproj

# Infrastructure посилається на Application та Domain
dotnet add src/MusicPlayer.Infrastructure/MusicPlayer.Infrastructure.csproj reference src/MusicPlayer.Application/MusicPlayer.Application.csproj
dotnet add src/MusicPlayer.Infrastructure/MusicPlayer.Infrastructure.csproj reference src/MusicPlayer.Domain/MusicPlayer.Domain.csproj

# Console посилається на всі рівні
dotnet add src/MusicPlayer.Console/MusicPlayer.Console.csproj reference src/MusicPlayer.Domain/MusicPlayer.Domain.csproj
dotnet add src/MusicPlayer.Console/MusicPlayer.Console.csproj reference src/MusicPlayer.Application/MusicPlayer.Application.csproj
dotnet add src/MusicPlayer.Console/MusicPlayer.Console.csproj reference src/MusicPlayer.Infrastructure/MusicPlayer.Infrastructure.csproj

# Tests посилаються на всі рівні
dotnet add tests/MusicPlayer.Tests/MusicPlayer.Tests.csproj reference src/MusicPlayer.Domain/MusicPlayer.Domain.csproj
dotnet add tests/MusicPlayer.Tests/MusicPlayer.Tests.csproj reference src/MusicPlayer.Application/MusicPlayer.Application.csproj
dotnet add tests/MusicPlayer.Tests/MusicPlayer.Tests.csproj reference src/MusicPlayer.Infrastructure/MusicPlayer.Infrastructure.csproj

echo "✓ Рішення успішно створено!"
echo "✓ Проекти: Domain, Application, Infrastructure, Console, Tests"
echo "✓ Всі посилання налаштовані"
echo ""
echo "Наступні кроки:"
echo "  cd MusicPlayer"
echo "  dotnet build"
echo "  dotnet test"
echo "  dotnet run --project src/MusicPlayer.Console/MusicPlayer.Console.csproj"
