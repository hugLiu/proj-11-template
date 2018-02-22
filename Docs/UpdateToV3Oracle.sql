--UpdateToV3Oracle:用于从已有2.0版本数据库升级到3.0的Oracle版本
/*增加options选项和逻辑删除字段*/
alter table Base_Article Add Options NUMBER default 0  not null;
alter table Base_Article Add IsDeleted NUMBER(1) default 0 not null ;
alter table Base_Catalog Add Options NUMBER default 0 not null ;
alter table Base_Catalog Add IsDeleted NUMBER(1)  default 0 not null;
alter table Base_CatalogExt Add Options NUMBER default 0 not null ;
alter table Base_CatalogExt Add IsDeleted NUMBER(1) default 0 not null;

alter table Base_ArticleExt modify Value NVARCHAR2(400);
/*将State字段中的删除标志移到逻辑删除字段*/
update Base_Article Set IsDeleted=1,State=State-1 WHERE BITAND(State, 1)=1;

/*变按位枚举为常量比较, oracle 暂时缺*/
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

/*增加索引优化性能* oralce missing*/
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