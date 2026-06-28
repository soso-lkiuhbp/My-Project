from app import api,r,db
from flask import request
from flask_restx import Resource
from app.models import *
import uuid 
from app.SMTPHandler import sendEmail
import string
import random
from app.TokenHandler import *

user_ns=api.namespace('api/user',description='用户操作')


@user_ns.route('/login')
class Login(Resource):
    def post(self):
        print('你成功请求')
        username=request.form.get('username')
        password=request.form.get('password')
        user=user_tbl.query.filter(user_tbl.username == username,user_tbl.password == password).first()
        if user:
            data = [user.to_dict()]
            token = create_token(username)
            data[0]['token'] = token
            return {"msg":"登陆成功","data":data,'code':1001}
        else:
            return {"msg":"用户名密码错误",'code':-1}
        
@user_ns.route('/auto-login')
class AutoLogin(Resource):
    def post(self):
        print("自动登录")
        username=request.form.get('username')
        token = request.form.get('token')
        result = decode_token(token)
        if result == 0:
            return {"msg":"token已过期",'code':0}
        elif result == -1:
            return {"msg":"token不可用",'code':-1}
        else:
            if username !=result['username']:
                return{"msg":"token不匹配",'code':-2}
            else:
                user=user_tbl.query.filter(user_tbl.username==username).first()
                if not user:
                    return {"msg":"用户不存在",'code':-1}
                data =[user.to_dict()]
                data[0]['token']=create_token(username)
                return{"msg":"登陆成功","data":data,'code':1001}

        
        
@user_ns.route('/register')
class Register(Resource):
    def post(self):
        print("注册")
        username = request.form.get('username')
        user = user_tbl.query.filter(user_tbl.username==username).first()
        if user:
            return{"msg":"用户名已注册","code":0}
        code = request.form.get('code')
        password=request.form.get('password')
        nickname=request.form.get('nickname')
        r_code = r.get(username).decode()
        if code ==r_code:
            id=str(uuid.uuid4())
            newuser=user_tbl(id=id,username=username,password=password,nickname=nickname)
            db.session.add(newuser)
            db.session.commit()
            return {"msg":"注册成功","code":1001}
        else:
            return{"msg":"验证码不正确","code":-1}
        

@user_ns.route('/sendcode')
class SendCode(Resource):
    def post(self):
        print("发送验证码")
        username = request.form.get('username')
        user=user_tbl.query.filter(user_tbl.username==username).first()
        if user:
            return{"msg":"用户已注册","code":0}
        
        code=self.generate_code()
       
        result = sendEmail(username,code)

        if result==1:
            r.set(username,code)

            return{"msg":"发送验证码成功","code":1001}
        elif result ==-1:
            return{"msg":"发送验证码失败","code":-1}
    def generate_code(self):
        code = ''
        characters=string.ascii_letters+string.digits
        for _ in range(5):
          code += random.choice(characters)
        return code
    

@user_ns.route('/delete_user')
class DeleteUser(Resource):
    def delete(self):
        username = request.form.get('username')
        
        # 查找用户
        user = user_tbl.query.filter(user_tbl.username == username).first()
        
        if not user:
            return {"msg": "用户不存在"}, 404
        
        # 删除用户
        db.session.delete(user)
        db.session.commit()
        
        # 同时删除Redis中的验证码缓存
        r.delete(username)
        
        return {"msg": f"用户 {username} 已删除"}, 200
      

      # 在 app/resources.py 中添加清理接口
@user_ns.route('/clear_cache')
class ClearCache(Resource):
    def delete(self):
        """清理Redis缓存"""
        try:
            # 获取所有key
            keys = r.keys('*')
            
            # 删除所有key
            for key in keys:
                r.delete(key)
            
            return {"msg": f"已清理 {len(keys)} 个缓存"}, 200
        except Exception as e:
            return {"msg": f"清理失败: {str(e)}"}, 500

@user_ns.route('/clear_user_cache/<string:username>')
class ClearUserCache(Resource):
    def delete(self, username):
        """清理指定用户的缓存"""
        try:
            result = r.delete(username)
            if result:
                return {"msg": f"已清理用户 {username} 的缓存"}, 200
            else:
                return {"msg": f"用户 {username} 没有缓存"}, 404
        except Exception as e:
            return {"msg": f"清理失败: {str(e)}"}, 500