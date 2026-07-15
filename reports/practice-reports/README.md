# Итоговый отчёт

Готовый LaTeX-проект отчётов по заданиям всех этапов учебной практики.

Тексты второго, третьего и итогового отчётов находятся в `sections/report02.tex`, `sections/report03.tex` и `sections/report04.tex`. Все файлы подключены в `main.tex`. Графики расположены в каталоге `images`, а исходные результаты заключительного этапа находятся в `results`.

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

Итоговый файл: `output/pdf/Итоговый_отчет_по_учебной_практике.pdf`.
