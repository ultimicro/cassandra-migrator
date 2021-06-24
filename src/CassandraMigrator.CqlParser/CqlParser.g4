/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014 by Domagoj Kovačević
 * Copyright (c) 2021 by Ultima Microsystems
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
 * associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
 * NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

parser grammar CqlParser;

options
   { tokenVocab = CqlLexer; }

root
   : cqls? MINUSMINUS? eof
   ;

cqls
   : (cql MINUSMINUS? statementSeparator | empty_)* (cql (MINUSMINUS? statementSeparator)? | empty_)
   ;

statementSeparator
   : SEMI
   ;

empty_
   : statementSeparator
   ;

cql
   : alterKeyspace
   | alterMaterializedView
   | alterRole
   | alterTable
   | alterType
   | alterUser
   | applyBatch
   | createAggregate
   | createFunction
   | createIndex
   | createKeyspace
   | createMaterializedView
   | createRole
   | createTable
   | createTrigger
   | createType
   | createUser
   | delete_
   | dropAggregate
   | dropFunction
   | dropIndex
   | dropKeyspace
   | dropMaterializedView
   | dropRole
   | dropTable
   | dropTrigger
   | dropType
   | dropUser
   | grant
   | insert
   | listPermissions
   | listRoles
   | revoke
   | select
   | truncate
   | update
   | use_
   ;

revoke
   : kwRevoke priviledge kwOn resource K_FROM role
   ;

listUsers
   : kwList kwUsers
   ;

listRoles
   : kwList kwRoles (kwOf role)? kwNorecursive?
   ;

listPermissions
   : kwList priviledge (kwOn resource)? (kwOf role)?
   ;

grant
   : kwGrant priviledge kwOn resource kwTo role
   ;

priviledge
   : (kwAll | kwAllPermissions)
   | kwAlter
   | kwAuthorize
   | kwDescibe
   | kwExecute
   | K_CREATE
   | kwDrop
   | kwModify
   | K_SELECT
   ;

resource
   : kwAll kwFunctions
   | kwAll kwFunctions kwIn kwKeyspace keyspace
   | kwFunction (keyspace DOT)? function_
   | kwAll kwKeyspaces
   | kwKeyspace keyspace
   | (K_TABLE)? table
   | kwAll kwRoles
   | kwRole role
   ;

createUser
   : K_CREATE kwUser ifNotExist? user K_WITH kwPassword stringLiteral (kwSuperuser | kwNosuperuser)?
   ;

createRole
   : K_CREATE kwRole ifNotExist? role roleWith?
   ;

createType
   : K_CREATE K_TYPE ifNotExist? udt syntaxBracketLr typeMemberColumnList syntaxBracketRr
   ;

typeMemberColumnList
   : column dataType (syntaxComma column dataType)*
   ;

createTrigger
   : K_CREATE kwTrigger ifNotExist? (keyspace DOT)? trigger kwUsing triggerClass
   ;

// CREATE MATERIALIZED VIEW

createMaterializedView
    : K_CREATE K_MATERIALIZED K_VIEW ifNotExist? table K_AS select kwPrimary kwKey syntaxBracketLr columnList syntaxBracketRr (K_WITH materializedViewOptions)?
    ;

materializedViewOptions
   : tableOptions
   | tableOptions K_AND clusteringOrder
   | clusteringOrder
   ;

createKeyspace
   : K_CREATE kwKeyspace ifNotExist? keyspace K_WITH kwReplication OPERATOR_EQ syntaxBracketLc replicationList syntaxBracketRc (K_AND durableWrites)?
   ;

createFunction
   : K_CREATE orReplace? kwFunction ifNotExist? (keyspace DOT)? function_ syntaxBracketLr paramList? syntaxBracketRr returnMode kwReturns dataType kwLanguage language K_AS codeBlock
   ;

codeBlock
   : CODE_BLOCK
   ;

paramList
   : param (syntaxComma param)*
   ;

returnMode
   : (kwCalled | kwReturns kwNull) kwOn kwNull kwInput
   ;

createAggregate
   : K_CREATE orReplace? kwAggregate ifNotExist? (keyspace DOT)? aggregate syntaxBracketLr dataType syntaxBracketRr kwSfunc function_ kwStype dataType kwFinalfunc function_ kwInitcond initCondDefinition
   ;

// paramList
// :
initCondDefinition
   : constant
   | initCondList
   | initCondListNested
   | initCondHash
   ;

initCondHash
   : syntaxBracketLc initCondHashItem (syntaxComma initCondHashItem)* syntaxBracketRc
   ;

initCondHashItem
   : hashKey COLON initCondDefinition
   ;

initCondListNested
   : syntaxBracketLr initCondList (syntaxComma constant | initCondList)* syntaxBracketRr
   ;

initCondList
   : syntaxBracketLr constant (syntaxComma constant)* syntaxBracketRr
   ;

orReplace
   : kwOr kwReplace
   ;

alterUser
   : kwAlter kwUser user K_WITH userPassword userSuperUser?
   ;

userPassword
   : kwPassword stringLiteral
   ;

userSuperUser
   : kwSuperuser
   | kwNosuperuser
   ;

alterType
   : kwAlter K_TYPE udt alterTypeOperation
   ;

alterTypeOperation
   : alterTypeAlterType
   | alterTypeAdd
   | alterTypeRename
   ;

alterTypeRename
   : kwRename alterTypeRenameList
   ;

alterTypeRenameList
   : alterTypeRenameItem (K_AND alterTypeRenameItem)*
   ;

alterTypeRenameItem
   : column kwTo column
   ;

alterTypeAdd
   : kwAdd column dataType (syntaxComma column dataType)*
   ;

alterTypeAlterType
   : kwAlter column K_TYPE dataType
   ;

alterTable
   : kwAlter K_TABLE table alterTableOperation
   ;

alterTableOperation
   : alterTableAdd
   | alterTableDropColumns
   | alterTableDropColumns
   | alterTableDropCompactStorage
   | alterTableRename
   | alterTableWith
   ;

alterTableWith
   : K_WITH tableOptions
   ;

alterTableRename
   : kwRename column kwTo column
   ;

alterTableDropCompactStorage
   : kwDrop kwCompact kwStorage
   ;

alterTableDropColumns
   : kwDrop alterTableDropColumnList
   ;

alterTableDropColumnList
   : column (syntaxComma column)*
   ;

alterTableAdd
   : kwAdd alterTableColumnDefinition
   ;

alterTableColumnDefinition
   : column dataType (syntaxComma column dataType)*
   ;

alterRole
   : kwAlter kwRole role roleWith?
   ;

roleWith
   : K_WITH (roleWithOptions (K_AND roleWithOptions)*)
   ;

roleWithOptions
   : kwPassword OPERATOR_EQ stringLiteral
   | kwLogin OPERATOR_EQ booleanLiteral
   | kwSuperuser OPERATOR_EQ booleanLiteral
   | kwOptions OPERATOR_EQ optionHash
   ;

alterMaterializedView
   : kwAlter K_MATERIALIZED K_VIEW table (K_WITH tableOptions)?
   ;

dropUser
   : kwDrop kwUser ifExist? user
   ;

dropType
   : kwDrop K_TYPE ifExist? udt
   ;

dropMaterializedView
   : kwDrop K_MATERIALIZED K_VIEW ifExist? table
   ;

dropAggregate
   : kwDrop kwAggregate ifExist? (keyspace DOT)? aggregate
   ;

dropFunction
   : kwDrop kwFunction ifExist? (keyspace DOT)? function_
   ;

dropTrigger
   : kwDrop kwTrigger ifExist? trigger kwOn table
   ;

dropRole
   : kwDrop kwRole ifExist? role
   ;

dropTable
   : kwDrop K_TABLE ifExist? table
   ;

dropKeyspace
   : kwDrop kwKeyspace ifExist? keyspace
   ;

dropIndex
   : kwDrop kwIndex ifExist? (keyspace DOT)? indexName
   ;

// CREATE TABLE

createTable
    : K_CREATE K_TABLE ifNotExist? table syntaxBracketLr columnDefinitionList syntaxBracketRr withElement?
    ;

withElement
    : K_WITH tableOptions? clusteringOrder?
    ;

clusteringOrder
    : kwClustering kwOrder kwBy syntaxBracketLr column orderDirection? syntaxBracketRr
    ;

tableOptions
    : tableOptionItem (K_AND tableOptionItem)*
    ;

tableOptionItem
    : ident OPERATOR_EQ tableOptionValue
    | ident OPERATOR_EQ optionHash
    ;

tableOptionValue
   : stringLiteral
   | floatLiteral
   ;

optionHash
   : syntaxBracketLc optionHashItem (syntaxComma optionHashItem)* syntaxBracketRc
   ;

optionHashItem
   : optionHashKey COLON optionHashValue
   ;

optionHashKey
   : stringLiteral
   ;

optionHashValue
   : stringLiteral
   | floatLiteral
   ;

columnDefinitionList
   : (columnDefinition) (syntaxComma columnDefinition)* (syntaxComma primaryKeyElement)?
   ;

//
columnDefinition
   : column dataType primaryKeyColumn?
   ;

//
primaryKeyColumn
   : kwPrimary kwKey
   ;

primaryKeyElement
   : kwPrimary kwKey syntaxBracketLr primaryKeyDefinition syntaxBracketRr
   ;

primaryKeyDefinition
   : singlePrimaryKey
   | compoundKey
   | compositeKey
   ;

singlePrimaryKey
   : column
   ;

compoundKey
   : partitionKey (syntaxComma clusteringKeyList)
   ;

compositeKey
   : syntaxBracketLr partitionKeyList syntaxBracketRr (syntaxComma clusteringKeyList)
   ;

partitionKeyList
   : (partitionKey) (syntaxComma partitionKey)*
   ;

clusteringKeyList
   : (clusteringKey) (syntaxComma clusteringKey)*
   ;

partitionKey
   : column
   ;

clusteringKey
   : column
   ;

applyBatch
   : kwApply kwBatch
   ;

beginBatch
   : kwBegin batchType? kwBatch usingTimestampSpec?
   ;

batchType
   : kwLogged
   | kwUnlogged
   ;

alterKeyspace
   : kwAlter kwKeyspace keyspace K_WITH kwReplication OPERATOR_EQ syntaxBracketLc replicationList syntaxBracketRc (K_AND durableWrites)?
   ;

replicationList
   : (replicationListItem) (syntaxComma replicationListItem)*
   ;

replicationListItem
   : STRING_LITERAL COLON STRING_LITERAL
   | STRING_LITERAL COLON DECIMAL_LITERAL
   ;

durableWrites
   : kwDurableWrites OPERATOR_EQ booleanLiteral
   ;

use_
   : kwUse keyspace
   ;

truncate
   : kwTruncate (K_TABLE)? table
   ;

createIndex
   : K_CREATE kwIndex ifNotExist? indexName? kwOn table syntaxBracketLr indexColumnSpec syntaxBracketRr
   ;

indexName
   : OBJECT_NAME
   | stringLiteral
   ;

indexColumnSpec
   : column
   | indexKeysSpec
   | indexEntriesSSpec
   | indexFullSpec
   ;

indexKeysSpec
   : kwKeys syntaxBracketLr OBJECT_NAME syntaxBracketRr
   ;

indexEntriesSSpec
   : kwEntries syntaxBracketLr OBJECT_NAME syntaxBracketRr
   ;

indexFullSpec
   : kwFull syntaxBracketLr OBJECT_NAME syntaxBracketRr
   ;

delete_
   : beginBatch? kwDelete deleteColumnList? fromSpec usingTimestampSpec? whereSpec (ifExist | ifSpec)?
   ;

deleteColumnList
   : (deleteColumnItem) (syntaxComma deleteColumnItem)*
   ;

deleteColumnItem
   : OBJECT_NAME
   | OBJECT_NAME LS_BRACKET (stringLiteral | decimalLiteral) RS_BRACKET
   ;

update
   : beginBatch? kwUpdate table usingTtlTimestamp? kwSet assignments whereSpec (ifExist | ifSpec)?
   ;

ifSpec
   : kwIf ifConditionList
   ;

ifConditionList
   : (ifCondition) (K_AND ifCondition)*
   ;

ifCondition
   : OBJECT_NAME OPERATOR_EQ constant
   ;

assignments
   : (assignmentElement) (syntaxComma assignmentElement)*
   ;

assignmentElement
   : OBJECT_NAME OPERATOR_EQ (constant | assignmentMap | assignmentSet | assignmentList)
   | OBJECT_NAME OPERATOR_EQ OBJECT_NAME (PLUS | MINUS) decimalLiteral
   | OBJECT_NAME OPERATOR_EQ OBJECT_NAME (PLUS | MINUS) assignmentSet
   | OBJECT_NAME OPERATOR_EQ assignmentSet (PLUS | MINUS) OBJECT_NAME
   | OBJECT_NAME OPERATOR_EQ OBJECT_NAME (PLUS | MINUS) assignmentMap
   | OBJECT_NAME OPERATOR_EQ assignmentMap (PLUS | MINUS) OBJECT_NAME
   | OBJECT_NAME OPERATOR_EQ OBJECT_NAME (PLUS | MINUS) assignmentList
   | OBJECT_NAME OPERATOR_EQ assignmentList (PLUS | MINUS) OBJECT_NAME
   | OBJECT_NAME syntaxBracketLs decimalLiteral syntaxBracketRs OPERATOR_EQ constant
   ;

assignmentSet
   : syntaxBracketLc (constant (syntaxComma constant)*)?  syntaxBracketRc
   ;

assignmentMap
   : syntaxBracketLc (constant syntaxColon constant) (constant syntaxColon constant)* syntaxBracketRc
   ;

assignmentList
   : syntaxBracketLs constant (syntaxComma constant)* syntaxBracketRs
   ;

assignmentTuple
   : syntaxBracketLr (
         constant ((syntaxComma constant)* | (syntaxComma assignmentTuple)*) |
         assignmentTuple (syntaxComma assignmentTuple)*
     ) syntaxBracketRr
   ;

insert
   : beginBatch? kwInsert kwInto table insertColumnSpec? insertValuesSpec ifNotExist? usingTtlTimestamp?
   ;

usingTtlTimestamp
   : kwUsing ttl
   | kwUsing ttl K_AND timestamp
   | kwUsing timestamp
   | kwUsing timestamp K_AND ttl
   ;

timestamp
   : kwTimestamp decimalLiteral
   ;

ttl
   : kwTtl decimalLiteral
   ;

usingTimestampSpec
   : kwUsing timestamp
   ;

ifNotExist
   : kwIf kwNot kwExists
   ;

ifExist
   : kwIf kwExists
   ;

insertValuesSpec
   : kwValues '(' expressionList ')'
   | kwJson constant
   ;

insertColumnSpec
   : '(' columnList ')'
   ;

columnList
   : column (syntaxComma column)*
   ;

expressionList
   : expression (syntaxComma expression)*
   ;

expression
   : constant
   | assignmentMap
   | assignmentSet
   | assignmentList
   | assignmentTuple
   ;

// SELECT

select
    : K_SELECT distinctSpec? kwJson? selectElements K_FROM table whereSpec? orderSpec? limitSpec? allowFilteringSpec?
    ;

allowFilteringSpec
   : kwAllow kwFiltering
   ;

limitSpec
   : kwLimit decimalLiteral
   ;

fromSpec
   : K_FROM table
   ;

orderSpec
   : kwOrder kwBy orderSpecElement
   ;

orderSpecElement
   : OBJECT_NAME (kwAsc | kwDesc)?
   ;

whereSpec
   : kwWhere relationElements
   ;

distinctSpec
   : kwDistinct
   ;

selectElements
   : (star = '*' | selectElement) (syntaxComma selectElement)*
   ;

selectElement
   : OBJECT_NAME '.' '*'
   | OBJECT_NAME (K_AS OBJECT_NAME)?
   | functionCall (K_AS OBJECT_NAME)?
   ;

relationElements
   : (relationElement) (K_AND relationElement)*
   ;

relationElement
    : ident (OPERATOR_EQ | OPERATOR_LT | OPERATOR_GT | OPERATOR_LTE | OPERATOR_GTE) constant
    | ident K_IS K_NOT K_NULL
    | functionCall (OPERATOR_EQ | OPERATOR_LT | OPERATOR_GT | OPERATOR_LTE | OPERATOR_GTE) constant
    | functionCall (OPERATOR_EQ | OPERATOR_LT | OPERATOR_GT | OPERATOR_LTE | OPERATOR_GTE) functionCall
    | OBJECT_NAME kwIn '(' functionArgs? ')'
    | '(' OBJECT_NAME (syntaxComma OBJECT_NAME)* ')' kwIn '(' assignmentTuple (syntaxComma assignmentTuple)* ')'
    | '(' OBJECT_NAME (syntaxComma OBJECT_NAME)* ')' (OPERATOR_EQ | OPERATOR_LT | OPERATOR_GT | OPERATOR_LTE | OPERATOR_GTE) ( assignmentTuple (syntaxComma assignmentTuple)* )
    | relalationContainsKey
    | relalationContains
    ;

relalationContains
   : OBJECT_NAME kwContains constant
   ;

relalationContainsKey
   : OBJECT_NAME (kwContains kwKey) constant
   ;

functionCall
   : OBJECT_NAME '(' STAR ')'
   | OBJECT_NAME '(' functionArgs? ')'
   ;

functionArgs
   : (constant | OBJECT_NAME | functionCall) (syntaxComma (constant | OBJECT_NAME | functionCall))*
   ;

constant
   : UUID
   | stringLiteral
   | decimalLiteral
   | floatLiteral
   | hexadecimalLiteral
   | booleanLiteral
   | codeBlock
   | kwNull
   ;

decimalLiteral
   : DECIMAL_LITERAL
   ;

floatLiteral
   : DECIMAL_LITERAL
   | FLOAT_LITERAL
   ;

stringLiteral
   : STRING_LITERAL
   ;

booleanLiteral
   : K_TRUE
   | K_FALSE
   ;

hexadecimalLiteral
   : HEXADECIMAL_LITERAL
   ;

// Data types

dataType
    : K_ASCII
    | K_BLOB
    | K_BOOLEAN
    | K_BIGINT
    | K_COUNTER
    | K_DATE
    | K_DECIMAL
    | K_DOUBLE
    | K_FLOAT
    | K_INET
    | K_INT
    | K_SMALLINT
    | K_TEXT
    | K_TIME
    | K_TIMESTAMP
    | K_TIMEUUID
    | K_TINYINT
    | K_UUID
    | K_VARCHAR
    | K_VARINT
    | K_LIST OPERATOR_LT dataType OPERATOR_GT
    | K_MAP OPERATOR_LT dataType COMMA dataType OPERATOR_GT
    | K_SET OPERATOR_LT dataType OPERATOR_GT
    | K_TUPLE OPERATOR_LT dataType (COMMA dataType)* OPERATOR_GT
    | K_FROZEN OPERATOR_LT dataType OPERATOR_GT
    | udt
    ;

// Identifiers

keyspace
    : ident
    ;

udt
    : (keyspace DOT)? ident
    ;

table
    : (keyspace DOT)? ident
    ;

column
    : ident
    ;

ident
    : IDENT
    | QUOTED_IDENT
    | keyword
    ;

// Keyword groups

keyword
    : K_LANGUAGE
    | K_TYPE
    ;

orderDirection
   : kwAsc
   | kwDesc
   ;

role
   : OBJECT_NAME
   ;

trigger
   : OBJECT_NAME
   ;

triggerClass
   : stringLiteral
   ;

aggregate
   : OBJECT_NAME
   ;

function_
   : OBJECT_NAME
   ;

language
   : OBJECT_NAME
   ;

user
   : OBJECT_NAME
   ;

password
   : stringLiteral
   ;

hashKey
   : OBJECT_NAME
   ;

param
   : paramName dataType
   ;

paramName
   : OBJECT_NAME
   ;

kwAdd
   : K_ADD
   ;

kwAggregate
   : K_AGGREGATE
   ;

kwAll
   : K_ALL
   ;

kwAllPermissions
   : K_ALL K_PERMISSIONS
   ;

kwAllow
   : K_ALLOW
   ;

kwAlter
   : K_ALTER
   ;

kwApply
   : K_APPLY
   ;

kwAsc
   : K_ASC
   ;

kwAuthorize
   : K_AUTHORIZE
   ;

kwBatch
   : K_BATCH
   ;

kwBegin
   : K_BEGIN
   ;

kwBy
   : K_BY
   ;

kwCalled
   : K_CALLED
   ;

kwClustering
   : K_CLUSTERING
   ;

kwCompact
   : K_COMPACT
   ;

kwContains
   : K_CONTAINS
   ;

kwDelete
   : K_DELETE
   ;

kwDesc
   : K_DESC
   ;

kwDescibe
   : K_DESCRIBE
   ;

kwDistinct
   : K_DISTINCT
   ;

kwDrop
   : K_DROP
   ;

kwDurableWrites
   : K_DURABLE_WRITES
   ;

kwEntries
   : K_ENTRIES
   ;

kwExecute
   : K_EXECUTE
   ;

kwExists
   : K_EXISTS
   ;

kwFiltering
   : K_FILTERING
   ;

kwFinalfunc
   : K_FINALFUNC
   ;

kwFull
   : K_FULL
   ;

kwFunction
   : K_FUNCTION
   ;

kwFunctions
   : K_FUNCTIONS
   ;

kwGrant
   : K_GRANT
   ;

kwIf
   : K_IF
   ;

kwIn
   : K_IN
   ;

kwIndex
   : K_INDEX
   ;

kwInitcond
   : K_INITCOND
   ;

kwInput
   : K_INPUT
   ;

kwInsert
   : K_INSERT
   ;

kwInto
   : K_INTO
   ;

kwIs
   : K_IS
   ;

kwJson
   : K_JSON
   ;

kwKey
   : K_KEY
   ;

kwKeys
   : K_KEYS
   ;

kwKeyspace
   : K_KEYSPACE
   ;

kwKeyspaces
   : K_KEYSPACES
   ;

kwLanguage
   : K_LANGUAGE
   ;

kwLimit
   : K_LIMIT
   ;

kwList
   : K_LIST
   ;

kwLogged
   : K_LOGGED
   ;

kwLogin
   : K_LOGIN
   ;

kwModify
   : K_MODIFY
   ;

kwNosuperuser
   : K_NOSUPERUSER
   ;

kwNorecursive
   : K_NORECURSIVE
   ;

kwNot
   : K_NOT
   ;

kwNull
   : K_NULL
   ;

kwOf
   : K_OF
   ;

kwOn
   : K_ON
   ;

kwOptions
   : K_OPTIONS
   ;

kwOr
   : K_OR
   ;

kwOrder
   : K_ORDER
   ;

kwPassword
   : K_PASSWORD
   ;

kwPrimary
   : K_PRIMARY
   ;

kwRename
   : K_RENAME
   ;

kwReplace
   : K_REPLACE
   ;

kwReplication
   : K_REPLICATION
   ;

kwReturns
   : K_RETURNS
   ;

kwRole
   : K_ROLE
   ;

kwRoles
   : K_ROLES
   ;

kwSet
   : K_SET
   ;

kwSfunc
   : K_SFUNC
   ;

kwStorage
   : K_STORAGE
   ;

kwStype
   : K_STYPE
   ;

kwSuperuser
   : K_SUPERUSER
   ;

kwTimestamp
   : K_TIMESTAMP
   ;

kwTo
   : K_TO
   ;

kwTrigger
   : K_TRIGGER
   ;

kwTruncate
   : K_TRUNCATE
   ;

kwTtl
   : K_TTL
   ;

kwUnlogged
   : K_UNLOGGED
   ;

kwUpdate
   : K_UPDATE
   ;

kwUse
   : K_USE
   ;

kwUser
   : K_USER
   ;

kwUsers
   : K_USERS
   ;

kwUsing
   : K_USING
   ;

kwValues
   : K_VALUES
   ;

kwWhere
   : K_WHERE
   ;

kwRevoke
   : K_REVOKE
   ;

eof
   : EOF
   ;

// BRACKETS
// L - left
// R - right
// a - angle
// c - curly
// r - rounded
syntaxBracketLr
   : LR_BRACKET
   ;

syntaxBracketRr
   : RR_BRACKET
   ;

syntaxBracketLc
   : LC_BRACKET
   ;

syntaxBracketRc
   : RC_BRACKET
   ;

syntaxBracketLs
   : LS_BRACKET
   ;

syntaxBracketRs
   : RS_BRACKET
   ;

syntaxComma
   : COMMA
   ;

syntaxColon
   : COLON
   ;
