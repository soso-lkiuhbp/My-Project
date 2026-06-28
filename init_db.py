from app import app, db
from app.models import user_tbl

with app.app_context():
    db.create_all()
    print("数据库表已创建！")
    print("表列表：")
    from sqlalchemy import inspect
    inspector = inspect(db.engine)
    tables = inspector.get_table_names()
    for table in tables:
        print(f"  - {table}")
