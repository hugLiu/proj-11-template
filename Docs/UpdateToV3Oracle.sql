--UpdateToV3Oracle:���ڴ�����2.0�汾���ݿ�������3.0��Oracle�汾
/*����optionsѡ����߼�ɾ���ֶ�*/
alter table Base_Article Add Options NUMBER default 0  not null;
alter table Base_Article Add IsDeleted NUMBER(1) default 0 not null ;
alter table Base_Catalog Add Options NUMBER default 0 not null ;
alter table Base_Catalog Add IsDeleted NUMBER(1)  default 0 not null;
alter table Base_CatalogExt Add Options NUMBER default 0 not null ;
alter table Base_CatalogExt Add IsDeleted NUMBER(1) default 0 not null;

alter table Base_ArticleExt modify Value NVARCHAR2(400);
/*��State�ֶ��е�ɾ����־�Ƶ��߼�ɾ���ֶ�*/
update Base_Article Set IsDeleted=1,State=State-1 WHERE BITAND(State, 1)=1;

/*�䰴λö��Ϊ�����Ƚ�, oracle ��ʱȱ*/
/*update base_article set options=2,state=state-4 from base_article a, Base_CatalogArticle ca
where ca.ArticleId = a.id
and ca.CatalogId = 21 and BITAND(state , 6) =6;

update base_article set options=1,state=state-32 from base_article a, Base_CatalogArticle ca
where ca.ArticleId = a.id
and ca.CatalogId = 21 and state & 34 = 34;

/*EventFinished*/
/*update base_article set state=2 from base_article a, Base_CatalogArticle ca
where ca.ArticleId = a.id
and ca.CatalogId = 21 and state & 258 = 258; */

/*EventRead*/
/*
update base_article set state=1 from base_article a, Base_CatalogArticle ca
where ca.ArticleId = a.id
and ca.CatalogId = 21 and state & 66 = 66;*/

/*���������Ż�����* oralce missing*/
/*CREATE NONCLUSTERED INDEX index_article ON Base_Article
(
  IsDeleted ASC,
  Id ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
CREATE NONCLUSTERED INDEX index_article2 ON Base_Article
(
  State ASC,
  EditorId ASC,
  IsDeleted ASC,
  Id ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX index_article3 ON Base_Article
(
  State ASC,
  EditorId ASC,
  IsDeleted ASC,
  Id ASC
)
INCLUDE (   Title,
  Options) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
  SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX index_article4 ON Base_Article
(
  Id ASC,
  Title ASC,
  Keywords ASC
)
INCLUDE (   Abstract,
  UrlTitle,
  Author,
  EditorId,
  CreateTime,
  EditTime,
  State,
  Clicks,
  Options,
  IsDeleted) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
CREATE NONCLUSTERED INDEX _dta_index_Base_ArticleExt_7_459148681__K3_K2 ON Base_ArticleExt
(
  CatlogExtId ASC,
  ArticleId ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
CREATE NONCLUSTERED INDEX index_articleext ON Base_ArticleExt
(
  CatlogExtId ASC
)
INCLUDE (   ArticleId) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX index_articleext2 ON Base_ArticleExt
(
  ArticleId ASC
)
INCLUDE (   Id,
  CatlogExtId,
  Value) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
SET ANSI_PADDING ON

CREATE NONCLUSTERED INDEX index_articleext3 ON Base_ArticleExt
(
  ArticleId ASC,
  CatlogExtId ASC,
  Value ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
CREATE NONCLUSTERED INDEX index_articlerelation ON Base_ArticleRelation
(
  SourceId ASC,
  TargetId ASC,
  RelationType ASC
)
INCLUDE (   Id,
  Ord) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON PRIMARY
*/