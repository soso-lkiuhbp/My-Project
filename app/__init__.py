from flask import Flask
from flask_restx import Api
from flask_sqlalchemy import SQLAlchemy
import json
import redis

with open('config.json') as config_file:
    config = json.load(config_file)

username=config['username']
password=config['password']
url=config['url']
database=config['database']
constr =f'mysql+pymysql://{username}:{password}@{url}/{database}'
app=Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI']=constr
db=SQLAlchemy(app)
r=redis.Redis(host='localhost',port=6379,db=0)
api=Api(app)
from app import resources