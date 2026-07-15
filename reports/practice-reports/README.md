# Промежуточные отчёты

Готовый LaTeX-проект отчётов по заданиям первой, второй и третьей недель практики.

Тексты второго и третьего отчётов находятся в `sections/report02.tex` и `sections/report03.tex` и подключены в `main.tex`. Графики третьей недели расположены в каталоге `images`.

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

Итоговый файл: `output/pdf/Промежуточные_отчеты_недели_1_2_и_3.pdf`.
