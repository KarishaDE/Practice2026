from pathlib import Path
import csv
import statistics

import matplotlib.pyplot as plt


root = Path(__file__).resolve().parent.parent
results = root / "results"
images = root / "images"
images.mkdir(parents=True, exist_ok=True)

with (results / "idle_cpu.csv").open(encoding="utf-8") as source:
    rows = list(csv.DictReader(source))

blocking = [float(row["blocking_wait_ms"]) for row in rows]
polling = [float(row["active_polling_ms"]) for row in rows]
means = [statistics.mean(blocking), statistics.mean(polling)]
errors = [statistics.stdev(blocking), statistics.stdev(polling)]

fig, ax = plt.subplots(figsize=(8.6, 4.8))
bars = ax.bar(
    ["Блокирующее ожидание", "Активный опрос"],
    means,
    yerr=errors,
    capsize=6,
    color=["#4472C4", "#ED7D31"],
    width=0.58,
)
ax.set_ylabel("Процессорное время за 1 с ожидания, мс")
ax.set_title("Затраты процессорного времени при отсутствии команд")
ax.set_ylim(0, max(means) * 1.1)
ax.grid(axis="y", alpha=0.3)
ax.set_axisbelow(True)
for bar, value in zip(bars, means):
    ax.text(
        bar.get_x() + bar.get_width() / 2,
        bar.get_height() + max(means) * 0.025,
        f"{value:.1f}",
        ha="center",
        va="bottom",
    )
fig.tight_layout()
fig.savefig(images / "week4-idle-cpu.png", dpi=180)
plt.close(fig)

with (results / "round_robin.csv").open(encoding="utf-8") as source:
    rows = list(csv.DictReader(source))

order = [int(row["order"]) for row in rows]
command_ids = [int(row["command_id"]) for row in rows]

fig, ax = plt.subplots(figsize=(8.6, 4.8))
ax.plot(order, command_ids, marker="o", linewidth=1.6, color="#4472C4")
ax.set_xlabel("Порядковый номер вызова Execute")
ax.set_ylabel("Идентификатор команды")
ax.set_title("Порядок выполнения пяти длительных команд")
ax.set_xticks(order)
ax.set_yticks(range(1, 6))
ax.grid(alpha=0.3)
ax.set_axisbelow(True)
fig.tight_layout()
fig.savefig(images / "week4-round-robin.png", dpi=180)
plt.close(fig)
