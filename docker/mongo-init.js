elysium = db.getSiblingDB('elysium');
elysium.createCollection('elysium');

elysium.elysium.createIndex({ "PrimaryKey": 1 }, { unique: true });
elysium.elysium.createIndex({ "ForeignKeys": 1 });

print("Indexes created on 'elysium' collection:");
printjson(elysium.elysium.getIndexes());

elysium = db.getSiblingDB('elysium');
elysium.createCollection('test');

elysium.test.createIndex({ "PrimaryKey": 1 }, { unique: true });
elysium.test.createIndex({ "ForeignKeys": 1 });

print("Indexes created on 'test' collection:");
printjson(elysium.test.getIndexes());