import sqlite3

create_table_query = """
CREATE TABLE IF NOT EXISTS urls (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    alias TEXT NOT NULL UNIQUE,
    fullname TEXT NOT NULL
);
"""

with sqlite3.connect("database.db") as conn:
    cursor = conn.cursor()
    cursor.execute(create_table_query)
    conn.commit()
