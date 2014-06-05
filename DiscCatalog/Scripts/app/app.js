var discCatalog = angular.module('discCatalogApp', ['ngGrid'])
var url = 'api/discs/Disc';

discCatalog.factory('discRepository', function($http) {
    return {
        getDiscs: function(callback) {
            $http.get(url).success(callback)
        },
        getDisc: function(callback, id) {
            $http.get(url + '/' + id).success(callback)
        },
        insertDisc: function(callback, disc) {
            $http.post(url, disc).success(callback)
        },
        updateDisc: function(callback, disc) {
            $http.put(url, disc).success(callback)
        },
        deleteDisc: function(callback, id) {
            $http.delete(url + '/' + id).success(callback)
        }
    }
});

discCatalog.controller('discCtrl', function($scope, discRepository) {
    getDiscs()

    function getDiscs() {
        discRepository.getDiscs(function(results) {
            $scope.discList = results
        })
        $scope.newDisc = {}
        $scope.action = 'New disc'
    }

    $scope.setScope = function(disc, action) {
        $scope.newDisc = { "Id": disc.Id, "Name": disc.Name, "Artist": disc.Artist, "Gener": disc.Gener, "Year": disc.Year }
        $scope.action = action
    }

    $scope.gridOptions = {
        data: 'discList',
        showGroupPanel: true,
        jqueryUITheme: true,
        columnDefs: [
            { field: 'Name', displayName: 'Name', width: '30%' },
            { field: 'Artist', displayName: 'Artist', width: '22%' },
            { field: 'Gener', displayName: 'Gener', width: '25%' },
            { field: 'Year', displayName: 'Year', width: '8%' },
            { displayName: 'Options', cellTemplate: '<input class="btn-primary" type="button" ng-click="setScope(row.entity,\'Edit\')" name="edit" value="Edit">&nbsp;<input class="btn-danger" type="button" ng-click="deleteDisc(row.entity.Id)" name="delete" value="Delete">', width: '15%' }
        ]
    }

    $scope.updateDisc = function() {
        if (!$scope.newDisc.Name) {
            alert('Disc name is blank')
        } else if (!$scope.newDisc.Artist) {
            alert('Artist is blank')
        } else {
            if ($scope.action == 'Edit') {
                discRepository.updateDisc(function() {
                    $scope.status = 'Disc updated successfully'
                    alert('Disc updated successfully')
                    getDiscs()
                }, $scope.newDisc)
            } else {
                discRepository.insertDisc(function() {
                    alert('Disc inserted successfully')
                    getDiscs()
                }, $scope.newDisc)
            }
            $scope.newDisc = {}
            $scope.action = 'New disc'
        }
    }

    $scope.deleteDisc = function(id) {
        discRepository.deleteDisc(function() {
            alert('Disc deleted')
            getDiscs()
        }, id)
    }

    $scope.cancel = function() {
        $scope.newDisc = {}
        $scope.action = 'New disc'
    }
});