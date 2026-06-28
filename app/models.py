from app import db


class user_tbl(db.Model):
    id=db.Column(db.String(255),primary_key=True)
    username=db.Column(db.String(255))
    password=db.Column(db.String(255))
    nickname=db.Column(db.String(255))

    def to_dict(self):
        return{
            'id':self.id,
            'username':self.username,
            'password':self.password,
            'nickname':self.nickname
        }