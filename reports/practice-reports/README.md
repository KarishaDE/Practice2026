# Промежуточные отчёты

Готовый LaTeX-проект отчётов по заданиям первой и второй недель практики.

Текст второго отчёта находится в `sections/report02.tex` и подключён в `main.tex`.

Перед отправкой укажите ФИО и группу в начале `main.tex`:

```tex
\newcommand{\StudentName}{Фамилия Имя Отчество}
\newcommand{\StudentGroup}{Название группы}
```

Сборка через MiKTeX:

```powershell
xelatex -interaction=nonstopmode -halt-on-error main.tex
xelatex -interaction=nonstopmode -halt-on-error main.tex
```

Итоговый файл: `output/pdf/Промежуточные_отчеты_недели_1_и_2.pdf`.
